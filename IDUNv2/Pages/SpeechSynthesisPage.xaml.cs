using IDUNv2.DataAccess;
using IDUNv2.Models;
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

namespace IDUNv2.Pages
{

    public sealed partial class SpeechSynthesisPage : Page
    {
        #region Fields

        private SpeechSynthesizer synthesizer;
        private ResourceContext speechContext;
        private ResourceMap speechResourceMap;

        #endregion

        #region CmdBar Actions

        private void ClearText(object param)
        {
            textBoxRead.Text = string.Empty;
        }

        private async void ReadText(object param)
        {
            MediaElement mediaElement = new MediaElement();
            SpeechSynthesisStream stream = await synthesizer.SynthesizeTextToStreamAsync(textBoxRead.Text);
            mediaElement.SetSource(stream, stream.ContentType);
            mediaElement.Play();
        }

        private void NavigateToLED(object param)
        {
            Frame.Navigate(typeof(LEDControlPage), null);
        }

        #endregion

        #region Constructors

        public SpeechSynthesisPage()
        {
            this.InitializeComponent();
            synthesizer = new SpeechSynthesizer();
            speechContext = ResourceContext.GetForCurrentView();
            speechContext.Languages = new string[] { SpeechSynthesizer.DefaultVoice.Language };
            speechResourceMap = ResourceManager.Current.MainResourceMap.GetSubtree("LocalizationTTSResources");
            InitializeListboxVoiceChooser();
        }

        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DAL.SetCmdBarItems(new CmdBarItem[]
            {
                new CmdBarItem(Symbol.Microphone, "Read Text", ReadText),
                new CmdBarItem(Symbol.Delete, "Clear Text", ClearText),
                new CmdBarItem(Symbol.Pin, "LED Control",NavigateToLED),
            });
        }

        private void InitializeListboxVoiceChooser()
        {
            var voices = SpeechSynthesizer.AllVoices;
            VoiceInformation currentVoice = synthesizer.Voice;
            foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Name = voice.DisplayName;
                item.Tag = voice;
                item.Content = voice.DisplayName + " (Language: " + voice.Language + ")";
                listBox.Items.Add(item);
                if (currentVoice.Id == voice.Id)
                {
                    item.IsSelected = true;
                    listBox.SelectedItem = item;
                }
            }
        }

        #region Event Handlers

        private void onSelectChange(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)(listBox.SelectedItem);
            VoiceInformation voice = (VoiceInformation)(item.Tag);
            synthesizer.Voice = voice;
        }

        private void TextForSpeech_GotFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(sender as TextBox);
        }

        private void TextForSpeech_LostFocus(object sender, RoutedEventArgs e)
        {
            DAL.ShowOSK(null);
        }

        #endregion
    }
}
