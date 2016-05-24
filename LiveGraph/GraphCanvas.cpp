#include "pch.h"
#include "GraphCanvas.h"
#include <assert.h>
#include <math.h>
#include <robuffer.h>

using namespace Platform;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Microsoft::WRL;
using namespace Windows::Storage::Streams;
using namespace LiveGraph;

static double dtime()
{
	static LARGE_INTEGER t0;
	static double invfreq;

	if (!invfreq) {
		LARGE_INTEGER f;
		QueryPerformanceFrequency(&f);
		invfreq = 1.0 / (double)f.QuadPart;
		QueryPerformanceCounter(&t0);
	}

	LARGE_INTEGER t;
	QueryPerformanceCounter(&t);
	return invfreq * (double)(t.QuadPart - t0.QuadPart);
}

static void ods(const char *fmt, ...)
{
	char buf[512];
	va_list ap;

	va_start(ap, fmt);
	vsprintf_s(buf, fmt, ap);
	OutputDebugStringA(buf);
	va_end(ap);
}

static uint32 rnd()
{
	static uint32 seed = 7;
	seed ^= seed << 13;
	seed ^= seed >> 17;
	seed ^= seed << 5;
	return seed;
}

void GraphCanvas::Clear()
{
	memset(pixels, 0x30, width*height * 4);
}

static int g_linetest(float p, float q, float *t)
{
	float r = q / p;
	if (p == 0.0f && q < 0.0f) return 0;
	if (p < 0.0f) {
		if (r > t[1]) return 0;
		else if (r > t[0]) t[0] = r;
	} else if (p > 0.0f) {
		if (r < t[0]) return 0;
		else if (r < t[1]) t[1] = r;
	}
	return 1;
}

static int g_clipline(float *x0, float *y0, float *x1, float *y1, float maxw, float maxh)
{
	float x = *x0;
	float y = *y0;
	float dx = *x1 - x;
	float dy = *y1 - y;
	float t[2] = { 0.0f, 1.0f };
	if (!g_linetest(-dx, x, t)) return 0;
	if (!g_linetest(dx, maxw - x, t)) return 0;
	if (!g_linetest(-dy, y, t)) return 0;
	if (!g_linetest(dy, maxh - y, t)) return 0;
	*x0 = x + dx*t[0];
	*y0 = y + dy*t[0];
	*x1 = x + dx*t[1];
	*y1 = y + dy*t[1];
	return 1;
}

void GraphCanvas::Line(float x0, float y0, float x1, float y1, int color)
{
	if (!g_clipline(&x0, &y0, &x1, &y1, (float)(width - 1), (float)(height - 1)))
		return;

	int ix0 = (int)x0;
	int ix1 = (int)x1;
	int iy0 = (int)y0;
	int iy1 = (int)y1;

	assert(ix0 >= 0 && ix0 < width);
	assert(ix1 >= 0 && ix1 < width);
	assert(iy0 >= 0 && iy0 < height);
	assert(iy1 >= 0 && iy1 < height);

	int dx = ix1 - ix0;
	int dy = iy1 - iy0;
	int e0 = dx > 0 ? 1 : -1;
	int e1 = e0;
	int step0 = dy > 0 ? width : -width;
	int step1 = 0;
	int i = dx > 0 ? dx : -dx;
	int j = dy > 0 ? dy : -dy;
	int d, n;

	if (j >= i)
		e1 = 0, step1 = step0, d = i, i = j, j = d;
	d = i / 2;
	step0 += e0;
	step1 += e1;
	n = i;

	int *p = (int *)pixels + iy0*width + ix0;
	do {
		*p = color;
		d += j;
		if (d >= i)
			d -= i, p += step0;
		else
			p += step1;
	} while (n--);
#if 0
	auto frac = [](float x) { return x - (float)(int)x; };
	auto rfrac = [frac](float x) { return 1.0f - frac(x); };
	auto plot = [this](int x, int y, float f) {
		int *p = (int *)this->pixels;
		int v = (int)(f*255.0f);
		int c = (v << 16) | (v << 8) | v;
		p[y*this->width + x] = c;
	};

	bool steep = fabsf(x1 - x0) < fabsf(y1 - y0);

	if (steep)
		std::swap(x0, y0), std::swap(x1, y1);
	if (x0 > x1)
		std::swap(x0, x1), std::swap(y0, y1);

	float dx = x1 - x0;
	float dy = y1 - y0;
	float m = dy / dx;

	float x = roundf(x0);
	float y = y0 + m*(x - x0);
	int px0 = (int)x;
	int py0 = (int)y;
	float xg = rfrac(x0 + 0.5f);

	if (steep)
		plot(py0, px0, rfrac(y)*xg), plot(py0 + 1, px0, frac(y) + xg);
	else
		plot(px0, py0, rfrac(y)*xg), plot(px0, py0 + 1, frac(y) + xg);

	float yc = y + m;

	x = roundf(x1);
	y = y1 + m*(x - x1);
	xg = rfrac(x1 + 0.5f);
	int px1 = (int)x;
	int py1 = (int)y;
	if (steep)
		plot(py1, px1, rfrac(y)*xg), plot(py1 + 1, px1, frac(y) + xg);
	else
		plot(px1, py1, rfrac(y)*xg), plot(px1, py1 + 1, frac(y) + xg);

	for (int i = px0 + 1; i <= px1 - 1; ++i) {
		int yi = (int)yc;
		float yf = frac(yc);
		float yr = rfrac(yc);
		if (steep)
			plot(yi, i, yr), plot(yi + 1, i, yf);
		else
			plot(i, yi, yr), plot(i, yi + 1, yf);
		yc += m;
	}
#endif
}

GraphCanvas::GraphCanvas(WriteableBitmap^ wb)
{
	ComPtr<IBufferByteAccess> bytes;
	reinterpret_cast<IInspectable*>(wb->PixelBuffer)->QueryInterface(IID_PPV_ARGS(&bytes));
	bytes->Buffer(&pixels);
	width = wb->PixelWidth;
	height = wb->PixelHeight;
}

String^ GraphCanvas::Foo()
{
	auto str = ref new String(L"LiveGraphDemo");
	return str;
}

void GraphCanvas::Draw(int left, float min, float max, const Platform::Array<uint32>^ points, int pointCount)
{
	auto getxy = [](uint32 p, int16& x, int16& y) {
		x = (int16)(p & 0xFFFF);
		y = (int16)(p >> 16);
	};

	Clear();

	{
		float range = (max - min);
		float halfh = (float)height * 0.5f;
		float ystep = halfh / range;
		float ymin = halfh + fabsf(min)*ystep;
		float ymax = halfh - max*ystep;
		float x0 = 0.0f;
		float x1 = (float)(width - 1);
		Line(x0, ymin, x1, ymin, 0x00AEFF);
		Line(x0, ymax, x1, ymax, 0xFF0000);
	}

	if (pointCount < 2)
		return;

	float fh = (float)height;
	for (int i = 1; i < pointCount; ++i) {
		int16 x0, y0, x1, y1;
		getxy(points[i - 1], x0, y0);
		getxy(points[i], x1, y1);
		float fy0 = fh - (float)y0;
		float fy1 = fh - (float)y1;
		Line(left+(float)x0, fy0, left+(float)x1, fy1, 0xFFFFFF);
	}
}

#if 0
struct Point {
	float x, y;
};
static Bag<Point, 256> dataPoints;

void GraphCanvas::Update()
{
	static double timer;
	static double tlast;
	static int fps;
	static float gx = width;
	static float gdx;
	static double sensorTimer;

	double t = dtime();
	double dt = t - tlast;
	tlast = t;

	double updateMs = dtime();

	Clear();

	int N = dataPoints.size;

	static const double tstep = 1.0 / 10.0;

	float dx = (float)width / (N >> 2);

	gdx = dt * width / (N*0.5*tstep);

	sensorTimer += dt;
	Point pt = { (float)width, 0.0f };
	while (sensorTimer >= tstep) {
		int rv = height / 2 + (int)(rnd() & 31) - 15;
		pt.x += gdx;
		pt.y = (float)rv;
		dataPoints.Add(pt);
		//pt.x -= dx;
		//dataPoints.Push(rv);
		sensorTimer -= tstep;
	}

	{
		int buf[PIPE_CAPACITY];

	}

	for (int i = 0; i < dataPoints.count; ++i) {
		auto dp = dataPoints.data[i];
		dp.x -= gdx;
		if (dp.x < 0)
			dataPoints.RemoveAt(i);
		else
			dataPoints.data[i] = dp;
#if 0
		unsigned ix = (int)dp.x;
		unsigned iy = (int)dp.y;
		for (int yy = 0; yy < 5; ++yy) {
			int yp = iy + yy;
			if ((unsigned)yp >= (unsigned)height)
				continue;
			for (int xx = 0; xx < 5; ++xx) {
				int xp = ix + xx;
				if ((unsigned)xp < (unsigned)width) {
					((int *)pixels)[yp*width + xp] = 0xFF0000;
				}
			}
		}
#endif
	}

	for (int i = 1; i < dataPoints.count; ++i) {
		auto d0 = dataPoints.data[i - 1];
		auto d1 = dataPoints.data[i];
		if (d0.x <= 0 || d1.x <= 0) {
			ods("%.2f, %.2f\n", d0.x, d1.x);
		}
		Line(d0.x, d0.y, d1.x, d1.y, 0xFFFFFF);
	}

	static bool move = true;
	if (dataPoints.count >= (width / dataPoints.size)*dx) {
		move = true;

	}
	if (move) {
		gx -= gdx*5.0f;
		if (gx < -(dataPoints.count*dx)) gx = (float)width;
	}

	updateMs = (dtime() - updateMs) * 1000.0;

	timer += dt;
	if (timer >= 1.0) {
		ods("%4d fps / %.2f ms (update: %.2f ms)\n", fps, dt*1000.0, updateMs);
		timer = 0.0;
		fps = 0;
	}

	++fps;
}
#endif