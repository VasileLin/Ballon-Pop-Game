using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Ballon_Pop_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer gameTimer = new DispatcherTimer();
        // set initial speed to 3
        int speed = 3;
        // set initial intervals to 90
        int intervals = 90;
        // create a new random number generator class
        Random rand = new Random();
        // a list to remove items from the canvas
        List<Rectangle> itemRemover = new List<Rectangle>();
        // background image texture class
        ImageBrush background = new ImageBrush();
        // integer to keep track of different balloon images
        int balloonSkins;
        // this integer will be used move the balloons left or right slightly
        int i;
        // missed balloon count integer
        int missedBalloons;
        // boolean to check if the game is active or not
        bool gameisactive;
        // score counter
        int score;
        // create a new media player to link the pop sound to
        private MediaPlayer player = new MediaPlayer();
        public MainWindow()
        {
            InitializeComponent();
            gameTimer.Tick += gameEngine; // link the timer to the game engine event
            gameTimer.Interval = TimeSpan.FromMilliseconds(20); // set timer interval to 20 milliseconds
            // set the background image for the canvas
            background.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/background-Image.jpg"));
            MyCanvas.Background = background;
            // run the reset game function
            resetGame();
        }

        private void canvasKeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space && gameisactive == false)
            {
                // run the reset function
                resetGame();
            }
        }

        private void popBalloons(object sender, MouseButtonEventArgs e)
        {
            // check i the game is active
            // this method will stop the player from the clicking the buttons when the game has stopped
            if (gameisactive)
            {
                if (e.OriginalSource is Rectangle)
                {
                    // if the click source is a rectangle then we will create a new rectangle
                    // and link it to the rectangle that sent the click event
                    Rectangle activeRec = (Rectangle)e.OriginalSource; // create the link between the sender rectangle

                    //load the mp3 file to the player URI
                    player.Open(new Uri("../../files/pop_sound.mp3", UriKind.RelativeOrAbsolute));
                    player.Play(); // play the mp3 file
                    MyCanvas.Children.Remove(activeRec); // find the rectangle and remove it from the canvas
                    score++; // add 1 to the score
                }
            }
        }

        private void startGame()
        {
            // this function will start the game
            // first start the timer
            gameTimer.Start();
            // set missed balloon to 0
            missedBalloons = 0;
            // set score to 0
            score = 0;
            // set intervals to 90
            intervals = 90;
            // change game is active boolean to true
            gameisactive = true;

            speed = 3; // set speed to 3
        }

        private void resetGame()
        {
            // this function will reset the game and remove any unused item from the canvas
            // run loop to find any rectangles in this canvas
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                // if found add it to the item remover list
                itemRemover.Add(x);
            }

            // check if there is any rectangles in the item remover list
            foreach (Rectangle y in itemRemover)
            {
                // if found remove it from the canvas
                MyCanvas.Children.Remove(y);
            }
            // clear everything form the list
            itemRemover.Clear();
            // start the game function
            startGame();
        }

        private void gameEngine(object sender, EventArgs e)
        {
            scoreLabel.Content = "Score: " + score; // link the score integer to the score label

            // deduct 10 from the intervals number
            intervals -= 10;
            // if interval is less than 1 then we will make new balloons for this game
            if (intervals < 1)
            {
                // first create a new image brush for the balloons called balloomImage
                ImageBrush balloonImage = new ImageBrush();
                // add 1 to the balloon skins integer
                balloonSkins += 1;

                // if the skins integer goes above 5 reset back to 1
                if (balloonSkins > 5)
                {
                    balloonSkins = 1;
                }

                // check which skin number is selected and change them to that number
                switch (balloonSkins)
                {
                    case 1:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon1.png"));
                        break;
                    case 2:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon2.png"));
                        break;
                    case 3:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon3.png"));
                        break;
                    case 4:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon4.png"));
                        break;
                    case 5:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon5.png"));
                        break;
                }

                // make a new rectangle called new balloon
                // inside this it has a tag called bloon, height 70 pixels and width 50 pixels and balloon image as the background
                Rectangle newBalloon = new Rectangle
                {
                    Tag = "bloon",
                    Height = 70,
                    Width = 50,
                    Fill = balloonImage
                };

                // set the top location of the balloon to 600 pixels so it spawns below the main window
                Canvas.SetTop(newBalloon, 600);
                // randomly set the left position between these two numbers
                Canvas.SetLeft(newBalloon, rand.Next(50, 400));
                // add the new item to the screen
                MyCanvas.Children.Add(newBalloon);
                // set the next interval between these two numbers
                intervals = rand.Next(80, 140);
            }

            //this is the main game loop
            // find any rectangles present in this canvas
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {

                // if the item is rectangle and has a tag bloon
                if (x is Rectangle && (string)x.Tag == "bloon")
                {
                    // randomly select between two numbers
                    i = rand.Next(-5, 5);
                    // move the balloon object towards top of the screen
                    Canvas.SetTop(x, Canvas.GetTop(x) - speed);
                    // randomly move the objects left and right position with i integer values
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - (i * -1));

                }

                // if the balloon reach top of the screen
                if (Canvas.GetTop(x) < 20)
                {
                    // remove them
                    itemRemover.Add(x);
                    // add 1 missed integer
                    missedBalloons++;
                }
            }

            // if you miss more than 10 balloons
            if (missedBalloons > 10)
            {
                gameisactive = false;// set game active to false
                gameTimer.Stop(); // stop the timer
                                  // show the message box below
                MessageBox.Show("Ai scapat 10 baloane,apasa Space pentru a reseta jocul");
            }

            // if score if more than 20
            if (score > 20)
            {
                // change the speed to 6
                speed = 6;
            }


            // garbage collection
            // remove any item thats been added to the item remover list
            foreach (Rectangle y in itemRemover)
            {
                MyCanvas.Children.Remove(y);
            }
        }
    }
}
