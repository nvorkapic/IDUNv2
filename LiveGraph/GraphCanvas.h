#pragma once

#include <atomic>

namespace LiveGraph {
	template <class T, int N>
	struct Bag {
		T data[N];
		int size = N;
		int count;

		bool Add(const T& elem)
		{
			if (count == size)
				return false;
			data[count++] = elem;
			return true;
		}

		bool RemoveAt(int i)
		{
			if (count == 0 || i >= count)
				return false;
			std::swap(data[count - 1], data[i]);
			--count;
			return true;
		}
	};

	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class GraphCanvas sealed  {
		static const int PIPE_CAPACITY = 128; // NOTE: must be power of 2

		template <class T>
		class Pipe {
			std::atomic<int> head;
			T data[PIPE_CAPACITY];
			std::atomic<int> tail;
		public:
			bool Write(const T& val) {
				int tl = tail.load(std::memory_order_relaxed);
				int tlnext = (tl + 1) & (PIPE_CAPACITY - 1);

				if (tlnext == head.load(std::memory_order_acquire))
					return false;

				data[tl] = val;
				tail.store(tlnext, std::memory_order_release);
				return true;
			}

			bool Read(T *out) {
				int hd = head.load(std::memory_order_relaxed);

				if (hd == tail.load(std::memory_order_acquire))
					return false;

				*out = data[hd];
				head.store((hd + 1) & (PIPE_CAPACITY - 1), std::memory_order_release);
				return true;
			}
		};

		byte *pixels;
		int width, height;

		Pipe<int> pipe;

		void Clear();
		void Line(float x0, float y0, float x1, float y1, int color);

	public:
		GraphCanvas(Windows::UI::Xaml::Media::Imaging::WriteableBitmap^ wb);

		Platform::String^ Foo();

		bool AddDataPoint(int data) {
			return pipe.Write(data);
		}

		void Draw(int left, float min, float max, const Platform::Array<uint32>^ points, int pointCount);

		//void Update();
	};
}