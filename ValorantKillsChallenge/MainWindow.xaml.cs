using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Path = System.IO.Path;
using TextBox = System.Windows.Controls.TextBox;

namespace ValorantKillsChallenge
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance;
        public static string valorantLogFilePath; // = @"C:\Users\%USERPROFILE%\AppData\Local\VALORANT\Saved\Logs\ShooterGame.log";
        private static int lastLineCount = 0;

        private static string animBluePrintLogLine = "AnimBlueprintLog: Warning: SLOTNODE: 'FullBody' in animation instance class TP_Core_AnimGraph_v2_C already exists.";
        private static string dmgHandlerPart = "LogDamageHandlerComponent";
        private static string trainingBotPart = "TrainingBot";

        public ViewModel viewModel;
        
        private const double ChallengeMilliseconds = 60000;

        KeyboardHook hook = new KeyboardHook();
        private bool registeredHotkey = false;

        bool firstReadDone = false;
        Task? currentTask = null;
        TimeSpan timeleft = TimeSpan.Zero;
        bool stopped = false;

        public MainWindow()
        {
            if(Instance == null) Instance = this;
            else this.Close();

            InitializeComponent();

            this.DataContext = new ViewModel();
            viewModel = this.DataContext as ViewModel;

            string LocalLowPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            valorantLogFilePath = Path.Combine(LocalLowPath, "VALORANT\\Saved\\Logs\\ShooterGame.log");

            registerRecordingHotkey(Keys.F5);

            run();
        }

        private void Button_Click(object sender, RoutedEventArgs e) => startChallenge();
        private void Button_Click_1(object sender, RoutedEventArgs e) => viewModel.KillCount++;
        private void Btn_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                btn.KeyUp -= Btn_KeyDown;
                btn.Content = e.Key.ToString();
            }

            hook.UnregisterHotkeys(); //gets rid of previous hotkey
            Keys newhotKey = (Keys)KeyInterop.VirtualKeyFromKey(e.Key);
            registerRecordingHotkey(newhotKey);
        }
        private void recordHotkeySet_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                btn.Content = "press key";
                btn.KeyUp += Btn_KeyDown;
            }
        }

        private void run()
        {
            var t = Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        var lines = ReadLines(() => File.Open(valorantLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Encoding.UTF8).ToList();
                        if (lines.Any())
                        {
                            if(firstReadDone)
                            {
                                var newLineCount = lines.Count;
                                var diff = newLineCount - lastLineCount;
                                if (diff >= 1)
                                {
                                    var newLines = lines.GetRange(newLineCount - diff, diff);
                                    if (newLines.Any(l => l.Contains(animBluePrintLogLine))) // l.Contains(dmgHandlerPart) && l.Contains(trainingBotPart)))
                                    {
                                        Instance.Dispatcher.Invoke(() =>
                                        {
                                            if (outputstacky.Children.Count > 10) outputstacky.Children.Clear();
                                            outputstacky.Children.Add(new TextBox() { Text = "BOT KILL DETECTED!", Foreground = Brushes.Green, Background = Brushes.Black });
                                            viewModel.KillCount++;
                                        });
                                    }
                                }
                            }
                            else
                            {
                                firstReadDone = true;
                            }
                            
                            lastLineCount = lines.Count;
                        }
                    }
                    catch (Exception ex)
                    {
                        Instance.Dispatcher.Invoke(() =>
                        {
                            outputstacky.Children.Add(new TextBox() { Text = ex.Message });
                        });
                    }
                    Thread.Sleep(100);
                }

            });
        }
        public IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
        {
            using (var stream = streamProvider())
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }
        private void startChallenge()
        {
            if(startbutton.Content == "Stop")
            {
                startbutton.Content = "Start";
                stopped = true;
                timeleft = TimeSpan.Zero;
                if (currentTask != null) currentTask = null;
                return;
            }

            startbutton.Content = "Stop";
            stopped = false;
            resultOutput.Text = "";
            viewModel.KillCount = 0;
            DateTime endTime = DateTime.Now.AddMilliseconds(ChallengeMilliseconds);

            currentTask = Task.Run(() =>
            {
                timeleft = endTime - DateTime.Now;
                while (!stopped && timeleft > TimeSpan.Zero)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        timerOutput.Text = $"{timeleft.Seconds}.{timeleft.Milliseconds}";
                    });

                    Thread.Sleep(10);
                    timeleft = endTime - DateTime.Now;
                }

                this.Dispatcher.Invoke(() =>
                {
                    timerOutput.Text = $"{0}.{000}";
                    resultOutput.Text = $"You got {viewModel.KillCount} kills!";
                    startbutton.Content = "Start";
                });
            });
        }
        public void registerRecordingHotkey(Keys newhotKey)
        {
            // register the event that is fired after the key press.
            if (!registeredHotkey) hook.KeyPressed += new EventHandler<KeyPressedEventArgs>(hook_KeyPressed);
            // register the control + alt + F12 combination as hot key.
            hook.RegisterHotKey(ModifierKeys.None, (Keys)newhotKey); //ModifierKeys.Control | ModifierKeys.Alt, Keys.F12
            registeredHotkey = true;
            recordHotkeySet.Content = newhotKey.ToString();
        }
        private void hook_KeyPressed(object sender, KeyPressedEventArgs e) => startChallenge();
    }
}
