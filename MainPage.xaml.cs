using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Testery
{
    public partial class MainPage : ContentPage
    {
        // Animations
        private List<Lottie.Forms.AnimationView> balls = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> pickedBalls = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> greenBeforeYellow = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> greenAfterYellow = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> redBeforeYellow = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> red1AfterYellow = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> red2AfterYellow = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> yellow = new List<Lottie.Forms.AnimationView>();
        private List<Lottie.Forms.AnimationView> otherAnims = new List<Lottie.Forms.AnimationView>();

        // Elements
        private ImageButton[] allChips = new ImageButton[13];
        private ImageButton[] decisionBtns = new ImageButton[3];
        private ImageButton imgButton;

        private bool allWasTaken = false;
        private bool halfWasTaken = false;
        private bool hasYellowArrived = false;
        private bool shieldActivated = false;
        private bool isRepeatPossible = false;

        private decimal[] multipliers = new decimal[21];
        private decimal allAmount = 0m;
        private decimal balance = 100m;
        private decimal betAmount = 0m;
        private decimal betStorage = 0m;
        private decimal betPreviousStep = 0m;
        private decimal continueAmount = 0m;
        private decimal currMultiplier = 0m;
        private decimal halfAmount = 0m;
        private decimal wonAmount = 0m;

        private int grBalls = 19;
        private int rdBalls = 8;
        private int selectedBall;
        private int step = 0;

        public MainPage()
        {
            InitializeComponent();

            imgButton = new ImageButton();
            imgButton.Clicked += _Chip020Button_Clicked;
            imgButton.Clicked += _Chip050Button_Clicked;
            imgButton.Clicked += _Chip1Button_Clicked;
            imgButton.Clicked += _Chip2Button_Clicked;
            imgButton.Clicked += _Chip5Button_Clicked;
            imgButton.Clicked += _Chip10Button_Clicked;
            imgButton.Clicked += _Chip25Button_Clicked;
            imgButton.Clicked += _Chip50Button_Clicked;
            imgButton.Clicked += _Chip100Button_Clicked;
            imgButton.Clicked += _ChipAllInButton_Clicked;
            imgButton.Clicked += _ChipUndoButton_Clicked;
            imgButton.Clicked += _ChipClearButton_Clicked;
            imgButton.Clicked += _ChipRepeatButton_Clicked;
            imgButton.Clicked += _TakeHalfButton_Clicked;
            imgButton.Clicked += _ContinueButton_Clicked;
            imgButton.Clicked += _TakeAllButton_Clicked;

            SetDefaultValues();
            SetBalls();
            SetChips();
            SetMultipliers();
            SetDecisionButtons();
            DecisionButtonsOff();
            CheckForZero();

            SetYellow();
            SetGreenAfterYellow();
            SetGreenBeforeYellow();
            SetRedBeforeYellow();
            SetRed1AfterYellow();
            SetRed2AfterYellow();
            SetOtherAnims();
        }


        // CLICKERS : Action
        private async void _PlayButton_Clicked(object sender, EventArgs e)
        {
            welcomeWord.IsVisible = false;
            TurnArrayOff(allChips);
            CheckForZero();
            SaveBet();
            PickBall();
            ColorCheckAndPlay();
            await Task.Delay(5000);
            UpdateCounter();
            ArrayAnimationsOff(greenBeforeYellow);
            ArrayAnimationsOff(greenAfterYellow);
            ArrayAnimationsOff(redBeforeYellow);
            ArrayAnimationsOff(red1AfterYellow);
            ArrayAnimationsOff(red2AfterYellow);
            ArrayAnimationsOff(yellow);
            ArrayAnimationsOff(otherAnims);
        }

        private void _ResetButton_Clicked(object sender, EventArgs e)
        {
            ResetBalls(ref balls, ref pickedBalls);
            RevertToDefalutState();
            ArrayAnimationsOff(otherAnims);
            ShowPlayResetButtons();
        }

        // CLICKERS : Chips
        private void ChipAction(decimal chipValue, ImageButton chip)
        {
            CheckButton(chipValue);
            CheckIfBetPossible(betPreviousStep, chip);
            CheckPossibleRep();
            DecreaseBalance();
            TurnImageButtonOn(chipUndo);
            CheckForZero();
            Vibration.Vibrate(100);
        }

        private void _Chip020Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(0.20m, chip020);
        }

        private void _Chip050Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(0.50m, chip050);
        }

        private void _Chip1Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(1m, chip1);
        }

        private void _Chip2Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(2m, chip2);
        }

        private void _Chip5Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(5m, chip5);
        }

        private void _Chip10Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(10m, chip10);
        }

        private void _Chip25Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(25m, chip25);
        }

        private void _Chip50Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(50m, chip50);
        }

        private void _Chip100Button_Clicked(object sender, EventArgs e)
        {
            ChipAction(100m, chip100);
        }

        private void _ChipAllInButton_Clicked(object sender, EventArgs e)
        {
            if (betAmount == 0m)
            {
                betAmount += balance;
                betPreviousStep = betAmount;
                CheckIfAllInPossible(betPreviousStep, chipAllIn);
            }
            else if(betAmount > 0m)
            {
                balance += betAmount;
                betAmount = balance;
                betPreviousStep = betAmount;
                CheckIfAllInPossible(betPreviousStep, chipAllIn);
            }
            DecreaseBalance();
            UndoONRepeatOFF();
            CheckForZero();
            Vibration.Vibrate(500);
        }

        // CLICKERS : Buttons
        private void _ChipUndoButton_Clicked(object sender, EventArgs e)
        {
            CheckBalance(chipUndo);
            if (betAmount > 0m && betStorage > 0m)
            {
                betAmount -= betPreviousStep;
                TurnImageButtonOn(chipRepeat);
            }
            else if(betAmount > 0m && chipRepeat.IsEnabled == false)
            {
                betAmount -= betPreviousStep;
            }
            else if (betAmount <= 0m)
            {
                betAmount = 0;
            }
            CheckDecimals(betAmount, betSize);
            TurnImageButtonOff(chipUndo);
            ReturnBalance();
            CheckForZero();
            CheckPossibleRep();
        }

        private void _ChipClearButton_Clicked(object sender, EventArgs e)
        {
            betPreviousStep = betAmount;
            betAmount = 0m;
            CheckDecimals(betAmount, betSize);
            ReturnBalance();
            if (balance > 0 || betAmount > 0)
            {
                TurnImageButtonOff(chipUndo);
                TurnRepeatOn();
                CheckForZero();
            }
            else{ }
        }

        private void _ChipRepeatButton_Clicked(object sender, EventArgs e)
        {
            CheckIfRepeatPossible();
            if (isRepeatPossible == true)
            {
                betAmount = betStorage;
                betPreviousStep = betAmount;
                CheckDecimals(betAmount, betSize);
                TurnImageButtonOff(chipRepeat);
                TurnImageButtonOn(chipUndo);
                CheckForZero();
                isRepeatPossible = false;
            }
        }

        private void _TakeHalfButton_Clicked(object sender, EventArgs e)
        {
            halfWasTaken = true;
            ArrayAnimationsOff(otherAnims);
            AddToWonAmount(halfAmount);
            TakeHalf();
            IncreaseBalance();
            DecisionButtonsOff();
            _PlayButton_Clicked(sender, e);
        }

        private void _ContinueButton_Clicked(object sender, EventArgs e)
        {
            halfWasTaken = false;
            ArrayAnimationsOff(otherAnims);
            DecisionButtonsOff();
            _PlayButton_Clicked(sender, e);
        }

        private async void _TakeAllButton_Clicked(object sender, EventArgs e)
        {
            halfWasTaken = false;
            allWasTaken = true;
            ArrayAnimationsOff(otherAnims);
            AddToWonAmount(allAmount);
            TakeAll();
            IncreaseBalance();
            DecisionButtonsOff();
            ShowWonAmount();
            await Task.Delay(6000);
            _ResetButton_Clicked(sender, e);
        }
       

        // KEY
        private void ColorCheckAndPlay()
        {
            TurnAnimationOff(playBtnAnim);   

            // If shield is INACTIVE
            if (shieldActivated == false)
            {
                // If came GREEN ball while shield is INACTIVE
                if (balls[selectedBall] == greenBall && !pickedBalls.Contains(yellowBall))
                {
                    CallAnimation(greenBeforeYellow, step);
                    UpdateGreenBalls();
                    CalculateWinnings(step, multipliers);
                    DecisionButtonsON();
                }

                // If came RED ball while shield is INACTIVE
                else if (balls[selectedBall] == redBall && !pickedBalls.Contains(yellowBall))
                {
                    CallAnimation(redBeforeYellow, step);
                    UpdateRedBalls();
                    CheckToShowWonAmount();
                }

                // If came YELLOW ball
                else if (balls[selectedBall] == yellowBall)
                {
                    hasYellowArrived = true;
                    CallAnimation(yellow, step);
                    ActivateShield();
                    AfterYellowButtonsON();
                }

                // If came GREEN ball and shield has been BROKEN
                else if (balls[selectedBall] == greenBall && pickedBalls.Contains(yellowBall) && pickedBalls.Contains(redBall))
                {
                    CallAnimation(greenAfterYellow, step);
                    UpdateGreenBalls();
                    CalculateWinnings(step, multipliers);
                    DecisionButtonsON();
                }

                // If came RED ball and shield has been BROKEN
                else if (balls[selectedBall] == redBall && pickedBalls.Contains(yellowBall) && pickedBalls.Contains(redBall))
                {
                    CallAnimation(red2AfterYellow, step);
                    UpdateRedBalls();
                    CheckToShowWonAmount();
                }
            }

            // If shield is ACTIVATED
            else if (shieldActivated == true)
            {
                // If came GREEN ball
                if (balls[selectedBall] == greenBall && pickedBalls.Contains(yellowBall))
                {
                    CallAnimation(greenAfterYellow, step);
                    UpdateGreenBalls();
                    AfterYellowButtonsON();
                }

                // If came RED ball
                else if (balls[selectedBall] == redBall && pickedBalls.Contains(yellowBall))
                {
                    CallAnimation(red1AfterYellow, step);
                    UpdateRedBalls();
                    CalculateWinnings(step, multipliers);
                    AfterYellowButtonsON();
                }
            }

            AddRemoveBall(ref balls, ref pickedBalls, ref selectedBall);
        }


        // ACTIONS
        private void AddRemoveBall(ref List<Lottie.Forms.AnimationView> list, ref List<Lottie.Forms.AnimationView> newList, ref int ball)
        {
            newList.Add(list[ball]);
            list.RemoveAt(ball);
        }

        private int PickBall()
        {
            Random random = new Random();
            int number = random.Next(0, balls.Count);
            selectedBall = number;
            return number;
        }

        private void ResetReplica()
        {
            ResetBalls(ref balls, ref pickedBalls);
            RevertToDefalutState();
            ArrayAnimationsOff(otherAnims);
            ShowPlayResetButtons();
        }

        private void ResetBalls(ref List<Lottie.Forms.AnimationView> list, ref List<Lottie.Forms.AnimationView> newList)
        {
            counter.Text = "0/20";
            list.AddRange(newList);
            newList.Clear();
        }

        private void ReduceRepeat()
        {
            balance+= betAmount;
            balance -= betStorage;
            CheckDecimals(balance, balanceBar);
        }

        private void SaveBet()
        {
            betStorage = betAmount;
        }


        // CALCULATIONS
        private void AddToWonAmount(decimal pickedOption)
        {
            wonAmount += pickedOption;
        }

        private void CalculateWinnings(int currStep, decimal[] array)
        {
            switch (hasYellowArrived)
            {
                case true:
                    halfAmount = betAmount * array[currStep] / 2m;
                    continueAmount = betAmount * array[currStep];
                    allAmount = betAmount * array[currStep];
                    break;
                case false:
                    halfAmount = betAmount * array[currStep + 1] / 2m;
                    continueAmount = betAmount * array[currStep + 1];
                    allAmount = betAmount * array[currStep + 1];
                    break;
                default:
                    
            }
            CheckDecimals(halfAmount, takeHalfAmountBar);
            CheckDecimals(continueAmount, continueAmountBar);
            CheckDecimals(allAmount, takeAllAmountBar);
        }

        private async void DecreaseBalance()
        {
            await Task.Delay(160);
            CheckDecimals(balance, balanceBar);
            balanceBar.TextColor = Color.FromHex("#F9715E");
            balanceBar.Opacity = .75;

            CheckDecimals(betAmount, betSize);
            betSize.TextColor = Color.FromHex("#5DEFBB");
            betSize.Opacity = .75;

            await Task.Delay(160);
            balanceBar.TextColor = Color.FromHex("#D7CCBD");
            betSize.TextColor = Color.FromHex("#E1FFBC");
            balanceBar.Opacity = 1;
            betSize.Opacity = 1;
        }

        private async void IncreaseBalance()
        {
            await Task.Delay(160);
            CheckDecimals(balance, balanceBar);
            balanceBar.TextColor = Color.FromHex("#5DEFBB");
            balanceBar.Opacity = .75;

            CheckDecimals(betAmount, betSize);
            betSize.TextColor = Color.FromHex("#F9715E");
            betSize.Opacity = .75;

            await Task.Delay(160);
            balanceBar.TextColor = Color.FromHex("#D7CCBD");
            betSize.TextColor = Color.FromHex("#E1FFBC");
            balanceBar.Opacity = 1;
            betSize.Opacity = 1;
        }

        private void ReduceBalance()
        {
            balance -= betPreviousStep;
            CheckDecimals(balance, balanceBar);
        }

        private void ReturnBalance()
        {
            balance += betPreviousStep;
            CheckDecimals(balance, balanceBar);
            IncreaseBalance();
            betPreviousStep = 0m;
        }

        private void TakeAll()
        {
            if (allWasTaken == true)
            {
                balance += allAmount;
                betAmount = 0m;
            }
        }

        private void TakeHalf()
        {
            if(halfWasTaken == true)
            {
                balance += halfAmount;
                betAmount /= 2m;
            }
        }


        // HELPERS
        private async void ActivateShield()
        {
            shieldActivated = true;
            await Task.Delay(6050);
            shieldStatus.Text = "ACTIVATED!";
            shieldStatus.TextColor = Color.FromHex("#EDDB7D");
        }

        private async void BrokeShield()
        {
            if (shieldActivated == false)
            {
                await Task.Delay(2200);
                shieldStatus.Text = "Broken!";
                shieldStatus.TextColor = Color.FromHex("#F9715E");
            }
        }

        private void CallAnimation(List<Lottie.Forms.AnimationView> animation, int index)
        {
            animation[index].IsEnabled = true;
            animation[index].IsVisible = true;
            animation[index].PlayAnimation();
        }

        private async void RunAnimation(List<Lottie.Forms.AnimationView> animation, int index)
        {
            switch (index)
            {
                case 1: // youWon
                    CallAnimation(animation, index);
                    await Task.Delay(2500);
                    wonAmountBar.IsVisible = true;
                    wonAmountBar.IsEnabled = true;
                    CheckDecimals(wonAmount, wonAmountBar);
                    TurnAnimationOff(playBtnAnim);
                    break;

                case 2: // activeShield
                    await Task.Delay(5200);
                    CallAnimation(animation, index);
                    await Task.Delay(1650);
                    contPlay.IsVisible = true;
                    continueAmountBar.IsVisible = false;
                    break;

                case 4: // staysShield
                    await Task.Delay(5200);
                    CallAnimation(animation, index);
                    contPlay.IsVisible = true;
                    continueAmountBar.IsVisible = false;
                    break;

                default:
                    CallAnimation(animation, index);
                    break;
            }
        }

        private void CheckDecimals(decimal number, Button button)
        {
            if (number %1 == 0)
            {
                button.Text = $"$ {number:0}";
            }
            else if (number %1 != 0)
            {
                button.Text = $"$ {number:0.00}";
            }
        }

        private void CheckForZero()
        {
            if (betAmount <= 0m || betSize.Text == null)
            {
                betAmount = 0m;
                CheckDecimals(betAmount, betSize);
                TurnAnimationOff(playBtnAnim);
                TurnAnimationOn(playBtnDisabledAnim);
            }
            else if (betAmount > 0m)
            {
                CheckDecimals(betAmount, betSize);
                TurnAnimationOff(playBtnDisabledAnim);
                TurnAnimationOn(playBtnAnim);
            }
        }

        private void CheckIfAllInPossible(decimal number, ImageButton button)
        {
            if (number > balance)
            {
                TurnImageButtonOff(button);
            }
            else if (number <= balance)
            {
                TurnImageButtonOn(button);
                betAmount += number - number;
                CheckDecimals(betAmount, betSize);
                ReduceBalance();
            }
        }

        private void CheckIfBetPossible(decimal number, ImageButton button)
        {
            if (number > balance)
            {
                betAmount += 0m;
                button.Opacity = 0.5;
            }
            else if (number <= balance)
            {
                TurnImageButtonOn(button);
                betAmount += number;
                CheckDecimals(betAmount, betSize);
                ReduceBalance();
            }
        }

        private void CheckBalance(ImageButton button)
        {
            if (balance <= 0m)
            {
                TurnImageButtonOff(button);
            }
        }

        private void CheckButton(decimal chipValue)
        {
            if (balance <= 0m || balance < chipValue)
            {
                betPreviousStep = 0m;
            }
            else
            {
                betPreviousStep = chipValue;
            }
        }

        private void CheckPossibleRep()
        {
            if (balance < betStorage)
            {
                TurnImageButtonOff(chipRepeat);
            }
        }

        private bool CheckIfRepeatPossible()
        {
            if (betStorage > 0m && balance >= betStorage)
            {
                isRepeatPossible = true;
                ReduceRepeat();
            }
            else
            {
                TurnImageButtonOff(chipRepeat);
            }
            return isRepeatPossible;
        }

        private void CheckRepeatAtStart()
        {
            if (betStorage > 0m && balance >= betStorage)
            {
                TurnImageButtonOn(chipRepeat);
            }
            else if (betStorage <= 0m || balance < betStorage)
            {
                betStorage = 0m;
                TurnImageButtonOff(chipRepeat);
            }
        }

        private async void CheckToShowWonAmount()
        {
            await Task.Delay(10);
            
            if (wonAmount > 0m)
            {
                await Task.Delay(5250);
                RunAnimation(otherAnims, 5);
                await Task.Delay(2000);
                RunAnimation(otherAnims, 1);
                await Task.Delay(6000);
                ResetReplica();
            }
            else
            {
                await Task.Delay(5250);
                RunAnimation(otherAnims, 6);
                await Task.Delay(2000);
                RunAnimation(otherAnims, 0);
            }
        }

        private async void ShowWonAmount()
        {
            await Task.Delay(10);
            RunAnimation(otherAnims, 1);
        }

        private void FindCurrentMultiplier(int currStep, ref decimal[] array)
        {
            currMultiplier = array[currStep+1];
            multiBar.Text = $"{currMultiplier}X";
            IncreaseMultiplierAnimation();
        }

        private async void ShowPlayResetButtons()
        {
            await Task.Delay(10);
            ResetButton.IsEnabled = false;
            PlayButton.IsEnabled = false;
            TurnAnimationOn(playBtnDisabledAnim);
        }

        private void UndoONRepeatOFF()
        {
            TurnImageButtonOn(chipUndo);
            TurnImageButtonOff(chipRepeat);
        }
       

        // SETUPS
        private void SetBalls()
        {
            // Balls at the start
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(redBall);
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(greenBall);
            balls.Add(redBall);
            balls.Add(greenBall);
            balls.Add(yellowBall);
        }

        private void SetChips()
        {
            allChips[0] = chip020;
            allChips[1] = chip050;
            allChips[2] = chip1;
            allChips[3] = chip2;
            allChips[4] = chip5;
            allChips[5] = chip10;
            allChips[6] = chip25;
            allChips[7] = chip50;
            allChips[8] = chip100;
            allChips[9] = chipAllIn;
            allChips[10] = chipUndo;
            allChips[11] = chipClear;
            allChips[12] = chipRepeat;
        }

        private void SetDecisionButtons()
        {
            decisionBtns[0] = takeHalf;
            decisionBtns[1] = contPlay;
            decisionBtns[2] = takeAll;
        }

        private void SetDefaultValues()
        {
            CheckDecimals(balance, balanceBar);
            CheckDecimals(betAmount, betSize);
            counter.Text = "0/20";
            shieldStatus.Text = "Deactivated";
            currMultiplier = 0m;
            multiBar.Text = $"{currMultiplier}X";
        }

        private void SetMultipliers()
        {
            multipliers[0] = 0m;
            multipliers[1] = 1.2m;
            multipliers[2] = 1.6m;
            multipliers[3] = 2m;
            multipliers[4] = 2.7m;
            multipliers[5] = 3.6m;
            multipliers[6] = 5m;
            multipliers[7] = 7.1m;
            multipliers[8] = 10m;
            multipliers[9] = 15m;
            multipliers[10] = 21.5m;
            multipliers[11] = 33m;
            multipliers[12] = 54m;
            multipliers[13] = 95m;
            multipliers[14] = 160m;
            multipliers[15] = 310m;
            multipliers[16] = 550m;
            multipliers[17] = 1200m;
            multipliers[18] = 2900m;
            multipliers[19] = 6800m;
            multipliers[20] = 18000m;
        }

        // SETUPS : Animations
        private void SetGreenBeforeYellow()
        {
            // Greens with yellow
            greenBeforeYellow.Add(greenBall);
            greenBeforeYellow.Add(green1Y);
            greenBeforeYellow.Add(green2Y);
            greenBeforeYellow.Add(green3Y);
            greenBeforeYellow.Add(green4Y);
            greenBeforeYellow.Add(green5Y);
            greenBeforeYellow.Add(green6Y);
            greenBeforeYellow.Add(green7Y);
            greenBeforeYellow.Add(green8Y);
            greenBeforeYellow.Add(green9Y);
            greenBeforeYellow.Add(green10Y);
            greenBeforeYellow.Add(green11Y);
            greenBeforeYellow.Add(green12Y);
            greenBeforeYellow.Add(green13Y);
            greenBeforeYellow.Add(green14Y);
            greenBeforeYellow.Add(green15Y);
            greenBeforeYellow.Add(green16Y);
            greenBeforeYellow.Add(green17Y);
            greenBeforeYellow.Add(green18Y);
            greenBeforeYellow.Add(green19Y);
        }

        private void SetGreenAfterYellow()
        {
            // Greens w/o yellow
            greenAfterYellow.Add(green0N);
            greenAfterYellow.Add(green1N);
            greenAfterYellow.Add(green2N);
            greenAfterYellow.Add(green3N);
            greenAfterYellow.Add(green4N);
            greenAfterYellow.Add(green5N);
            greenAfterYellow.Add(green6N);
            greenAfterYellow.Add(green7N);
            greenAfterYellow.Add(green8N);
            greenAfterYellow.Add(green9N);
            greenAfterYellow.Add(green10N);
            greenAfterYellow.Add(green11N);
            greenAfterYellow.Add(green12N);
            greenAfterYellow.Add(green13N);
            greenAfterYellow.Add(green14N);
            greenAfterYellow.Add(green15N);
            greenAfterYellow.Add(green16N);
            greenAfterYellow.Add(green17N);
            greenAfterYellow.Add(green18N);
            greenAfterYellow.Add(green19N);
        }

        private void SetRedBeforeYellow()
        {
            // Reds with yellow
            redBeforeYellow.Add(redBall);
            redBeforeYellow.Add(redG1Y);
            redBeforeYellow.Add(redG2Y);
            redBeforeYellow.Add(redG3Y);
            redBeforeYellow.Add(redG4Y);
            redBeforeYellow.Add(redG5Y);
            redBeforeYellow.Add(redG6Y);
            redBeforeYellow.Add(redG7Y);
            redBeforeYellow.Add(redG8Y);
            redBeforeYellow.Add(redG9Y);
            redBeforeYellow.Add(redG10Y);
            redBeforeYellow.Add(redG11Y);
            redBeforeYellow.Add(redG12Y);
            redBeforeYellow.Add(redG13Y);
            redBeforeYellow.Add(redG14Y);
            redBeforeYellow.Add(redG15Y);
            redBeforeYellow.Add(redG16Y);
            redBeforeYellow.Add(redG17Y);
            redBeforeYellow.Add(redG18Y);
            redBeforeYellow.Add(redG19Y);
        }

        private void SetRed1AfterYellow()
        {
            // Reds w/o yellow
            red1AfterYellow.Add(redG1N);
            red1AfterYellow.Add(redG2N);
            red1AfterYellow.Add(redG3N);
            red1AfterYellow.Add(redG4N);
            red1AfterYellow.Add(redG5N);
            red1AfterYellow.Add(redG6N);
            red1AfterYellow.Add(redG7N);
            red1AfterYellow.Add(redG8N);
            red1AfterYellow.Add(redG9N);
            red1AfterYellow.Add(redG10N);
            red1AfterYellow.Add(redG11N);
            red1AfterYellow.Add(redG12N);
            red1AfterYellow.Add(redG13N);
            red1AfterYellow.Add(redG14N);
            red1AfterYellow.Add(redG15N);
            red1AfterYellow.Add(redG16N);
            red1AfterYellow.Add(redG17N);
            red1AfterYellow.Add(redG18N);
            red1AfterYellow.Add(redG19N);
        }

        private void SetRed2AfterYellow()
        {
            // Reds after broken shield
            red2AfterYellow.Add(redG1B);
            red2AfterYellow.Add(redG2B);
            red2AfterYellow.Add(redG3B);
            red2AfterYellow.Add(redG4B);
            red2AfterYellow.Add(redG5B);
            red2AfterYellow.Add(redG6B);
            red2AfterYellow.Add(redG7B);
            red2AfterYellow.Add(redG8B);
            red2AfterYellow.Add(redG9B);
            red2AfterYellow.Add(redG10B);
            red2AfterYellow.Add(redG11B);
            red2AfterYellow.Add(redG12B);
            red2AfterYellow.Add(redG13B);
            red2AfterYellow.Add(redG14B);
            red2AfterYellow.Add(redG15B);
            red2AfterYellow.Add(redG16B);
            red2AfterYellow.Add(redG17B);
            red2AfterYellow.Add(redG18B);
            red2AfterYellow.Add(redG19B);
        }

        private void SetYellow()
        {
            yellow.Add(yellowBall);
            yellow.Add(yellowG1);
            yellow.Add(yellowG2);
            yellow.Add(yellowG3);
            yellow.Add(yellowG4);
            yellow.Add(yellowG5);
            yellow.Add(yellowG6);
            yellow.Add(yellowG7);
            yellow.Add(yellowG8);
            yellow.Add(yellowG9);
            yellow.Add(yellowG10);
            yellow.Add(yellowG11);
            yellow.Add(yellowG12);
            yellow.Add(yellowG13);
            yellow.Add(yellowG14);
            yellow.Add(yellowG15);
            yellow.Add(yellowG16);
            yellow.Add(yellowG17);
            yellow.Add(yellowG18);
            yellow.Add(yellowG19);
        }

        private void SetOtherAnims()
        {
            otherAnims.Add(tryAgain);
            otherAnims.Add(youWon);
            otherAnims.Add(activeShield);
            otherAnims.Add(brokenShield);
            otherAnims.Add(staysShield);
            otherAnims.Add(crash);
            otherAnims.Add(crashStays);
            otherAnims.Add(playBtnAnim);
            otherAnims.Add(playBtnDisabledAnim);
        }


        // SWITCHERS : On
        private async void AfterYellowButtonsON()
        {
            if (betAmount > 0m && balls[selectedBall] == yellowBall)
            {
                RunAnimation(otherAnims, 2);
            }
            else if (betAmount > 0 && balls[selectedBall] == greenBall)
            {
                RunAnimation(otherAnims, 4);
            }
            else if (betAmount > 0m && balls[selectedBall] == redBall)
            {
                await Task.Delay(5200);
                RunAnimation(otherAnims, 3);
                DecisionButtonsON();
            }
        }

        private async void DecisionButtonsON()
        {
            switch (hasYellowArrived)
            {
                case true:
                    await Task.Delay(2500);
                    if (betAmount > 0m && halfAmount >= 0.40m)
                    {
                        TurnArrayOn(decisionBtns);
                        TurnImageButtonOn(takeHalf);
                        takeHalfAmountBar.IsVisible = true;
                        continueAmountBar.IsVisible = true;
                        takeAllAmountBar.IsVisible = true;
                        hasYellowArrived = false;
                    }
                    else if (betAmount > 0m && halfAmount < 0.40m)
                    {
                        TurnArrayOn(decisionBtns);
                        TurnImageButtonOff(takeHalf);
                        takeHalfAmountBar.IsVisible = false;
                        continueAmountBar.IsVisible = true;
                        takeAllAmountBar.IsVisible = true;
                        hasYellowArrived = false;
                    }
                    break;

                case false:
                    if (betAmount > 0m && halfAmount >= 0.40m)
                    {
                        await Task.Delay(5200);
                        TurnArrayOn(decisionBtns);
                        TurnImageButtonOn(takeHalf);
                        takeHalfAmountBar.IsVisible = true;
                        continueAmountBar.IsVisible = true;
                        takeAllAmountBar.IsVisible = true;
                    }
                    else if (betAmount > 0m && halfAmount < 0.40m)
                    {
                        await Task.Delay(5200);
                        TurnArrayOn(decisionBtns);
                        TurnImageButtonOff(takeHalf);
                        takeHalfAmountBar.IsVisible = false;
                        continueAmountBar.IsVisible = true;
                        takeAllAmountBar.IsVisible = true;
                    }
                    break;
                default:
            }
        }

        private void TurnAnimationOn(Lottie.Forms.AnimationView animation)
        {
            animation.IsVisible = true;
            animation.IsEnabled = true;
            animation.Opacity = 1;
        }

        private void TurnArrayOn(ImageButton[] array)
        {
            foreach (var item in array)
            {
                item.IsVisible = true;
            }
        }

        private void TurnImageButtonOn(ImageButton imgButton)
        {
            imgButton.IsEnabled = true;
            imgButton.IsVisible = true;
            imgButton.Opacity = 1;
        }

        private void TurnRepeatOn()
        {
            if (balance > 0 && betStorage != 0)
            {
                TurnImageButtonOn(chipRepeat);
            }
            else if (balance <= 0 || betStorage == 0)
            {
                TurnImageButtonOff(chipRepeat);
            }
        }
      
        // SWITCHERS : Off
        private void ArrayAnimationsOff(List<Lottie.Forms.AnimationView> animations)
        {
            foreach (var item in animations)
            {
                item.IsEnabled = false;
                item.IsVisible = false;
            }
        }

        private void DecisionButtonsOff()
        {
            TurnArrayOff(decisionBtns);
            takeHalfAmountBar.IsVisible = false;
            continueAmountBar.IsVisible = false;
            takeAllAmountBar.IsVisible = false;
        }

        private void TurnAnimationOff(Lottie.Forms.AnimationView animation)
        {
            animation.IsVisible = false;
            animation.IsEnabled = false;
        }

        private void TurnArrayOff(ImageButton[] array)
        {
            foreach (var item in array)
            {
                item.IsVisible = false;
            }
        }

        private void TurnImageButtonOff(ImageButton button)
        {
            button.IsEnabled = false;
            button.Opacity = .5;
        }


        // UPDATERS
        private async void ClearBet()
        {
            CheckDecimals(betAmount, betSize);
            betSize.TextColor = Color.FromHex("#F9715E");
            betSize.Opacity = .75;

            await Task.Delay(160);

            balanceBar.TextColor = Color.FromHex("#D7CCBD");
            betSize.TextColor = Color.FromHex("#E1FFBC");
            balanceBar.Opacity = 1;
            betSize.Opacity = 1;
        }
                
        private async void IncreaseMultiplierAnimation()
        {
            await Task.Delay(160);
            multiBar.TextColor = Color.FromHex("#5DEFBB");
            multiBar.Opacity = .75;

            await Task.Delay(160);
            multiBar.Opacity = 1;
            multiBar.TextColor = Color.FromHex("#E7FDCA");
        }

        private void RevertToDefalutState()
        {
            CheckRepeatAtStart();
            ArrayAnimationsOff(otherAnims);

            grBalls = 19;
            rdBalls = 8;

            currMultiplier = 0m;
            multiBar.Text = $"{currMultiplier}X";
            step = 0;
            counter.TextColor = Color.FromHex("#D7CCBD");
            counter.Text = $"{step}/20";

            betAmount = 0m;
            wonAmount = 0m;
            wonAmountBar.IsEnabled = false;
            wonAmountBar.IsVisible = false;
            halfWasTaken = false;
            allWasTaken = false;
            welcomeWord.IsVisible = true;
            hasYellowArrived = false;

            shieldStatus.Text = "Deactivated";
            shieldStatus.TextColor = Color.FromHex("#8C8578");
            TurnImageButtonOff(chipUndo);
            TurnArrayOn(allChips);
            CheckForZero();
        }

        private async void UpdateCounter()
        {
            await Task.Delay(10);
            if (pickedBalls[pickedBalls.Count-1] == greenBall || pickedBalls[pickedBalls.Count-1] == yellowBall)
            {
                FindCurrentMultiplier(step, ref multipliers);
                step++;
                counter.Text = $"{step}/20";
            }
            else { };
        }

        private async void UpdateGreenBalls()
        {
            await Task.Delay(5000);
            if (grBalls >= 1)
            {
                grBalls--;
            }
            else if (grBalls == 0)
            {
                counter.Text = "JACKPOT";
                counter.TextColor = Color.FromHex("#D7CCBD");
            }
        }

        private async void UpdateRedBalls()
        {
            await Task.Delay(5000);
            // If red came before yellow or broken shield
            if (shieldActivated == false)
            {
                rdBalls--;
                counter.Text = "CRASH";
                counter.TextColor = Color.FromHex("#863024");
                betAmount = 0m;
                ClearBet();
                multiBar.Text = $"0X";
                ResetButton.IsEnabled = true;
            }
            // If 1st red came after yellow
            else if (shieldActivated == true)
            {
                rdBalls--;
                shieldActivated = false;
                BrokeShield();
            }
        }
    }
}
