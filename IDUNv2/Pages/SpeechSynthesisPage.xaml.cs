using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IDUNv2.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpeechSynthesisPage : Page
    {
        private SpeechSynthesizer synthesizer;

        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;

        public SpeechSynthesisPage()
        {
            this.InitializeComponent();

            synthesizer = new SpeechSynthesizer();

            keyboard.RegisterTarget(textBoxRead);

            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };

            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");

            InitializeListboxVoiceChooser();
        }


        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxRead.Text = string.Empty;
        }

        private async void btnRead_Click(object sender, RoutedEventArgs e)
        {
            MediaElement mediaElement = new MediaElement();

            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(textBoxRead.Text);

            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private void InitializeListboxVoiceChooser()
        {
            // Get all of the installed voices.
            var voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            VoiceInformation currentVoice = synthesizer.Voice;

            foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = voice.DisplayName;
                item.Tag = voice;
                item.Content = voice.DisplayName + " (Language: " + voice.Language + ")";
                listBox.Items.Add(item);

                // Check to see if we're looking at the current voice and set it as selected in the listbox.
                if (currentVoice.Id == voice.Id)
                {
                    item.IsSelected = true;
                    listBox.SelectedItem = item;
                }
            }
        }

        private async void onSelectChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(listBox.SelectedItem);
            VoiceInformation voice = (VoiceInformation)(item.Tag);
            synthesizer.Voice = voice;

            MediaElement mediaElement = new MediaElement();

            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync("Hello!");

            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private void TextForSpeech_GotFocus(object sender, RoutedEventArgs e)
        {
            keyboard.Visibility = Visibility.Visible;
        }

        private void TextForSpeech_LostFocus(object sender, RoutedEventArgs e)
        {
            keyboard.Visibility = Visibility.Collapsed;
        }
    }
}
