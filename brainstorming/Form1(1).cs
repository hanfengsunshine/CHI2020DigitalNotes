using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Drawing.Imaging;
using AForge.Imaging;
using InputManager;
using AForge.Imaging.Filters;
using AForge;
using System.Timers;

namespace SDGundamScript
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.ptrConsole = FindWindow("ConsoleWindowClass", null);
            SetWindowPos(ptrConsole, 0, 1049, 0, 320, 519, SWP_NOZORDER | SWP_SHOWWINDOW);
            SetForegroundWindow(ptrConsole);

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new System.Drawing.Point(1049, 520);

            Trace.Listeners.Clear();
            //Path.Combine(Path.GetTempPath(), AppDomain.CurrentDomain.FriendlyName)
            TextWriterTraceListener twtl = new TextWriterTraceListener("log.txt");
            twtl.Name = "TextLogger";
            twtl.TraceOutputOptions = TraceOptions.ThreadId | TraceOptions.DateTime;

            ConsoleTraceListener ctl = new ConsoleTraceListener(false);
            ctl.TraceOutputOptions = TraceOptions.DateTime;

            Trace.Listeners.Add(twtl);
            Trace.Listeners.Add(ctl);
            Trace.AutoFlush = true;

        }

        //********************************************* 逻辑层 *********************************************
        private void createRoomButton_Click(object sender, EventArgs e)
        {
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);
            SetWindowPos(ptrGame, 0, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

            create_room_and_run(); //支持断线重连
        }

        public static Thread main;
        public static int offline_time = 0;
        private void restartButton_Click(object sender, EventArgs e)
        {
            while (true)
            {
                main = new Thread(restart_game);
                main.Start();
                // TODO: 增加掉线检测、GM检测、房间来人检测
                while (!isLoadServerSuccess)
                    Thread.Sleep(5000);

                while (true)
                {
                    //掉线检测
                    this.ptrGame = FindWindow("_GONLINE_", null);
                    if (ptrGame == IntPtr.Zero || isGameError())
                    {
                        isLoadServerSuccess = false;
                        offline_time++;
                        log("检测到掉线，正在重启游戏");
                        main.Abort();

                        Process[] processlist = Process.GetProcessesByName("GOnline");
                        foreach (Process theprocess in processlist)
                        {
                            theprocess.Kill();
                        }

                        break;
                    }

                    //任务失败检测
                    //log("检测到掉线，正在重启游戏");
                    //main.Abort();

                    Thread.Sleep(5000);
                }
            }
        }


        public bool isGameError()
        {
            IntPtr hWnd = IntPtr.Zero;
            foreach (Process pList in Process.GetProcesses())
            {
                if (pList.MainWindowTitle.Contains("发现错误"))
                {
                    hWnd = pList.MainWindowHandle;
                }
            }
            if (hWnd != IntPtr.Zero)
            {
                SetWindowPos(hWnd, 0, 477, 255, 0, 0, SWP_NOSIZE | SWP_NOZORDER | SWP_SHOWWINDOW);

                SetForegroundWindow(hWnd);
                Thread.Sleep(1000);
                mouse_click(887, 532, 800);
                return true;
            }
            return false;
        }


        public void clear_flag()
        {
            shotEnoughTime = 0;
            notExistEnemyCount = 0;
            isShotDown = true;
        }

        public static bool isFirstEnemy = true;
        public void findFirstEnemy()
        {
            //turn(90);
            Keyboard.KeyPress(Keys.W, 50);
            Thread.Sleep(300);
            //spurt(15);
            Keyboard.KeyDown(Keys.W);
            for (int i = 0; i < 90; i += 10)
            {

                Mouse.MoveRelative(0 + i, 0);
                Thread.Sleep(300);
            }
            Keyboard.KeyUp(Keys.W);
            Thread.Sleep(300);
            //Keyboard.KeyPress(Keys.A, 50);
            //Thread.Sleep(300);
            //Keyboard.KeyDown(Keys.A);
            Keyboard.KeyPress(Keys.D, 50);
            Thread.Sleep(300);
            Keyboard.KeyDown(Keys.D);
            Keyboard.KeyDown(Keys.D); //资深难度
            for (int i = 0; i < 70; i += 10) //资深难度
            {

                Mouse.MoveRelative(0 - i, 0);
                Thread.Sleep(100);
            }

            //for (int i = 0; i < 100; i += 10)
            //{

            //    Mouse.MoveRelative(0 + i, 0);
            //    Thread.Sleep(100);
            //}
            Keyboard.KeyUp(Keys.D); //资深难度
            Keyboard.KeyUp(Keys.A);
            //Mouse.PressButton(Mouse.MouseKeys.Right, 400);
            Thread.Sleep(100);
            Mouse.MoveRelative(50, -50);
            Thread.Sleep(100);
            active_auto_sight();
            isFirstEnemy = true;
        }

        public void active_auto_sight()
        {
            Mouse.PressButton(Mouse.MouseKeys.Right, 400);
        }

        public static Thread runningGame;
        private void testButton_Click(object sender, EventArgs e)
        {
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);
            SetWindowPos(ptrGame, 0, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            Thread.Sleep(1000);
            //turn(-150);
            //spurt(10);
            //step_right();
            //step_backword();

            //for (int i = 0; i < 180; i += 10)
            //{
            //    Mouse.MoveRelative(0 - i, 0);
            //    Thread.Sleep(100);
            //}

            while(true)
            {
                log(isScanEnemy() + "");
                Thread.Sleep(1000);
            }
            

            //findFirstEnemy();

            //if (isStayTooClose())
            //    step_backword();

            //spurt(5);
            //turn_left();
            //enter_mission();
            //等待结算界面
            //while (!isMatchEnd())
            //{
            //    log("还未进入结算页面");
            //    Thread.Sleep(500);
            //    Keyboard.KeyPress(Keys.Escape, 500);
            //    Thread.Sleep(500);
            //    Keyboard.KeyPress(Keys.Escape, 500);

            //    Mouse.PressButton(Mouse.MouseKeys.Right, 800);
            //    Thread.Sleep(500);
            //    Mouse.PressButton(Mouse.MouseKeys.Right, 800);
            //}
            //nextMatchOK = true;
            //start_working();
            //log(isOtherInRoom().ToString());
        }

        private void captureButton_Click(object sender, EventArgs e)
        {
            string[] p = textBox2.Text.Split(',');
            // debug button
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);
            Thread.Sleep(1000);
            extract_number_image(capture_save_screen(Int32.Parse(p[0]), Int32.Parse(p[1]), Int32.Parse(p[2]), Int32.Parse(p[3])));

            //Keyboard.ShortcutKeys(new Keys[] { Keys.LControlKey, Keys.LShiftKey, Keys.M }, 1000);
            //Thread.Sleep(2000);
            //mouse_dclick(523, 241);

        }

        private void test2_Click(object sender, EventArgs e)
        {
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);

            Thread.Sleep(3000);

            enter_mission();

        }

        //********************************************* 控制层 *********************************************
        //******************** 流程控制 ********************
        public void start_working() //刷多局任务，支持任务失败识别，不支持断线重连
        {
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);
            SetWindowPos(ptrGame, 0, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

            while (!isEnterRoom())
            {
                mouse_dclick(738, 648, 0);//focus the window
                Keyboard.KeyPress(Keys.Escape, 800);

                // 创房
                while (!isCreateRoom())
                {
                    mouse_click(797, 531, 800);
                    Thread.Sleep(1000);
                }

                while (true)
                {
                    mouse_click(511, 341); //选任务
                    Thread.Sleep(900);
                    if (isMission())
                        break;

                }

                mouse_click(578, 341);
                Thread.Sleep(800);
                SendKeys.SendWait(RandomString(4)); //设定密码
                Thread.Sleep(800);
                Keyboard.KeyPress(Keys.LShiftKey, 800);
                Thread.Sleep(800);
                mouse_click(474, 451, 800); //确认

                for (int i = 0; i < 5; i++)
                {
                    if (isEnterRoom())
                        break;
                    else
                        Thread.Sleep(3000);
                }
            }
            //mouse_click(89, 349); //选择难度
            //Thread.Sleep(800);

            // 无限开刷

            while (true)
            {
                isEnterMission = false;
                runningGame = new Thread(enter_mission);
                runningGame.Start();

                while (!isEnterMission)
                    Thread.Sleep(1000);

                while (true)
                {
                    //任务失败检测
                    Thread.Sleep(5000);
                    if (isEnterRoom())
                    {
                        if (kaguaiTimer.Enabled == true)
                            log("任务已失败");
                        else
                        {
                            log("任务已完成");
                            successtime++;
                        }

                        kaguaiTimer.Enabled = false;
                        isShotDown = true;
                        if (t_attack != null) t_attack.Join();
                        //if (t_checkShotDown != null) t_checkShotDown.Join();
                        if (t_shanqiang != null) t_shanqiang.Join();

                        if (runningGame != null) runningGame.Abort();
                        if (t_attack != null) t_attack.Abort();
                        //if (t_checkShotDown != null) t_checkShotDown.Abort();
                        if (t_shanqiang != null) t_shanqiang.Abort();

                        //discoverTimer.Enabled = false;
                        runtime++;
                        log(string.Format("脚本已经运行{0}局任务, 成功执行任务{1}局, 掉线{2}次，期间GM查房{3}次", runtime, successtime, offline_time, foundGMtime));
                        break;
                    }
                }
            }
            //while (true)
            //{
            //    enter_mission();
            //}
        }

        public static bool isLoadServerSuccess = false;
        public void restart_game()
        {
            // 启动游戏
            Process.Start("F:\\Downloads\\Compressed\\山东矮达客户端1130版\\山东登录器.exe");
            Thread.Sleep(5000);
            mouse_dclick(923, 581); //点击启动游戏

            log("正在启动游戏");

            Thread.Sleep(2000);
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);
            SetWindowPos(ptrGame, 0, dx(0), dy(0), 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);
            isLoadServerSuccess = true;

            Thread.Sleep(1000);
            // 启动完成
            while (!isLoginLoaded())
            {
                log("还未进入输入密码页面");
                mouse_click(515, 434, 800);//focus the window
                Thread.Sleep(500);
                Keyboard.KeyDown(Keys.Escape);
                Thread.Sleep(500);
                Keyboard.KeyUp(Keys.Escape);
                Thread.Sleep(1000);
                this.ptrGame = FindWindow("_GONLINE_", null);
                SetForegroundWindow(ptrGame);
            }

            log("已进入输入密码页面");
            // 输密码
            while (!isLoginSuccess2())
            {
                //mouse_click(520, 359);
                //Thread.Sleep(800);
                //Keyboard.KeyDown(Keys.Escape);
                //Thread.Sleep(800);
                //SendKeys.SendWait(ACCOUNT);

                while (true)
                {
                    mouse_click(482, 379);
                    Thread.Sleep(800);
                    Keyboard.KeyPress(Keys.Escape);
                    Thread.Sleep(800);
                    SendKeys.SendWait(PASSWORD);
                    log("正在输入密码");
                    Thread.Sleep(1000);
                    mouse_dclick(515, 434);
                    Keyboard.KeyPress(Keys.Enter);

                    if (!isPopLoginWindow())
                        break;
                }


                // loading 界面
                Thread.Sleep(2000);
                if (isLoginSuccess1())
                {
                    log("进入loading界面");
                    // 大天使号界面
                    Thread.Sleep(1500);
                    while (!isLoginSuccess2())
                    {
                        log("进入选择频道界面");
                        Thread.Sleep(1000);
                        //TODO: 退出重开
                    }
                    break;
                }
            }

            // 进入任务频道
            mouse_click(204, 596);
            Thread.Sleep(1000);


            MousePoint[] mission_server = new MousePoint[] {
                new MousePoint( 835, 437 ), //高二
                new MousePoint( 809, 394 ), //高一
                new MousePoint( 830, 249 ), //初一
                new MousePoint( 840, 290 ), //初二
            };

            int count = 0;
            while (true)
            {
                log("正在进入任务频道");
                mouse_click(mission_server[count++ % mission_server.Length], 800); // 选择频道
                Thread.Sleep(1000);
                Keyboard.KeyPress(Keys.Enter, 1000);
                Thread.Sleep(3500);
                if (isEnterMissionServer())
                    break;
            }
            start_working();
        }

        public void create_room_and_run()
        {
            while (!isEnterMissionServer())
            {
                Thread.Sleep(1000);
            }

            while (true)
            {
                main = new Thread(start_working);
                main.Start();
                // TODO: 增加掉线检测、GM检测、房间来人检测
                Thread.Sleep(5000);

                while (true)
                {
                    //！！！不支持，掉线检测
                    this.ptrGame = FindWindow("_GONLINE_", null);
                    if (ptrGame == IntPtr.Zero || isGameError())
                    {
                        log("检测到掉线，正在重启游戏");
                        main.Abort();

                        Process[] processlist = Process.GetProcessesByName("GOnline");
                        foreach (Process theprocess in processlist)
                        {
                            theprocess.Kill();
                        }

                        break;
                    }
                    Thread.Sleep(5000);
                }
            }
        }

        private System.Timers.Timer kaguaiTimer = new System.Timers.Timer(302000);
        private static bool isEnterMission = false;
        public static int foundGMtime = 0;
        public void enter_mission() //开始单局任务，不含任务失败检测
        {
            clear_flag();

            while (true)
            {
                if (!nextMatchOK || !isEnterRoom())
                    Thread.Sleep(1000);
                log("已经进入房间");
                break;
            }


            while (true)
            {

                Keyboard.KeyPress(Keys.Escape, 500);
                mouse_click(769, 657); //点击开始游戏
                nextMatchOK = false;

                if (isMatchStart())
                    break;
                Thread.Sleep(1000);
                log("还未进入任务，继续等待");

                if (isOtherInRoom())
                {
                    log("房间有GM/其他人，赶紧退房！");
                    foundGMtime++;
                    mouse_click(1005, 46, 800);
                    create_room_and_run();
                    return;
                }

                //充电
                charge();
            }
            log("已进入任务，一击马斯！");
            kaguaiTimer.Elapsed += kaGuaiOverTimerEvent;
            kaguaiTimer.Enabled = true;

            while (!isMatchStart2())
            {
                //ESC
                Keyboard.KeyPress(Keys.Escape, 300);
            }
            isEnterMission = true;

            Thread.Sleep(500);
            turn_up();
            Thread.Sleep(300);

            findFirstEnemy();
            discover();
            //discoverTimer.Elapsed += discoverTimerEvent;
            //discoverTimer.AutoReset = true;
            //discoverTimer.Enabled = true;
            //discoverTimer.Start();
            //while (true)
            //{
            //    if (discoverTimer.Enabled)
            //    {
            //        Thread.Sleep(3000);
            //        log("探索任务还未结束");
            //    }
            //    else
            //    {
            //        log("已经打够，可以开始卡怪");
            //        break;
            //    }
            //}


        }

        //******************** 操作控制 ********************

        public void charge()
        {
            Bitmap image = capture_screen(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            red_filter.Red = new IntRange(75, 120);
            red_filter.Green = new IntRange(0, 10);
            red_filter.Blue = new IntRange(30, 60);
            red_filter.ApplyInPlace(image);

            var blobs = extractBlobs(image, 7, 2);
            if (blobs.Length == 0)
                log("还不需要充电");
            else
            {

                var x = Convert.ToInt32(blobs[0].CenterOfGravity.X);
                var y = Convert.ToInt32(blobs[0].CenterOfGravity.Y);
                mouse_click(x, y);
                Thread.Sleep(900);
                mouse_click(456, 372, 800);
                Keyboard.KeyPress(Keys.Enter, 800);
                log("充电完毕");
            }
        }
        public void turn_up()
        {
            for (int i = 0; i < 60; i += 20)
            {
                Mouse.MoveRelative(0, 0 - i);
                Thread.Sleep(50);
            }
        }

        public void turn(double angleofline)
        {

            if (angleofline > 0)
            {
                for (int i = 0; i < angleofline; i += 20)
                {
                    Mouse.MoveRelative(0 + i, 0);
                    Thread.Sleep(50);
                }

            }
            else
            {
                for (int i = 0; i > angleofline; i -= 20)
                {
                    Mouse.MoveRelative(0 + i, 0);
                    Thread.Sleep(50);
                    Mouse.MoveRelative(0 + i, 0);

                }
            }
            Mouse.PressButton(Mouse.MouseKeys.Right, 400); //辅助瞄准帮助定位角度
        }

        public void turn_right()
        {
            for (int i = 0; i < 120; i += 20)
            {
                Mouse.MoveRelative(0 + i, 0);
                Thread.Sleep(50);
            }
        }

        public void turn_left()
        {
            for (int i = 0; i < 120; i += 20)
            {
                Mouse.MoveRelative(0 - i, 0);
                Thread.Sleep(50);
            }
        }

        public void spurt(double distance)
        {
            Keyboard.KeyPress(Keys.W, 50);
            Thread.Sleep(300);
            Keyboard.KeyPress(Keys.W, Convert.ToInt32(distance * 80));
            if (isStayTooClose())
                step_backword();
            //Thread.Sleep(800);
        }

        public void step_backword()
        {
            Keyboard.KeyPress(Keys.S, 1000);
        }

        public void step_right()
        {
            Keyboard.KeyPress(Keys.D, 900);
        }

        public void attack()
        {
            //log("attack 开始");
            while (!isShotDown)
            {
                Keyboard.KeyPress(Keys.D2, 800);
                Thread.Sleep(100);
                //Mouse.ButtonDown(Mouse.MouseKeys.Left);
                while (!isBulletEmpty())
                {
                    Mouse.PressButton(Mouse.MouseKeys.Left, 300);
                    Thread.Sleep(100);
                    //Thread.Sleep(1000);
                    if (isShotDown)
                        return;
                }

                //Mouse.ButtonUp(Mouse.MouseKeys.Left);

                Keyboard.KeyPress(Keys.D1, 800);
                Mouse.PressButton(Mouse.MouseKeys.Left, 300);
                Thread.Sleep(100);
                Mouse.PressButton(Mouse.MouseKeys.Left, 300);
                Thread.Sleep(100);
                //mouse_click(0, 0, 450);
                //Thread.Sleep(300);
            }
            //log("attack 结束");
        }

        public void shanQiang()
        {
            //log("shanqiang 开始");
            while (!isShotDown)
            {
                if (isStayTooClose())
                {
                    Keyboard.KeyPress(Keys.S, 400);
                    Thread.Sleep(300);
                }
                Keyboard.KeyPress(Keys.A, 400);
                Thread.Sleep(300);
                Keyboard.KeyPress(Keys.S, 400);
                Thread.Sleep(300);
                Keyboard.KeyPress(Keys.D, 400);
                Thread.Sleep(300);
            }
            //log("shanqiang 结束");
        }

        //********************************************* 方法工具 *******************************************

        //******************** 核心识别 ********************
        public static int notExistEnemyCount = 0;
        public bool isScanEnemy()  //自动瞄准扫到敌军
        {
            Thread.Sleep(600);
            Bitmap before_sight = capture_screen(459, 399, 41, 13);
            active_auto_sight();
            Bitmap after_sight = capture_screen(459, 399, 41, 13);
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.9F);
            var res = tm.ProcessImage(before_sight, after_sight);
            if (res.Length == 0)
                return true;
            return false;
        }

        public bool isExistEnemy()  //已瞄准识别
        {
            Bitmap sight_lf = capture_screen(458, 348, 28, 24); //左上绿准星
            Bitmap sight_rg = capture_screen(548, 348, 26, 25);

            ImageStatistics stat = new ImageStatistics(extract_sight(sight_lf));    //统计被过滤后还剩多少绿像素
            var sight_lf_count = (stat.GreenWithoutBlack.TotalCount);
            Thread.Sleep(200);

            stat = new ImageStatistics(extract_sight(sight_rg));
            var sight_rg_count = (stat.GreenWithoutBlack.TotalCount);

            //if (sight_lf_count > 20 && sight_rg_count > 20 && Math.Abs(sight_lf_count - sight_rg_count)<200)
            if (sight_lf_count > 0 && sight_rg_count > 0)
            {

                log(string.Format("左瞄准:{0}, 右瞄准：{1}", sight_lf_count.ToString("0.00"), sight_rg_count.ToString("0.00")));
                //log(Math.Abs(sight_lf_count - sight_rg_count).ToString());
                return true;
            }
            notExistEnemyCount++;
            return false;
        }

        public AForge.Point getLargestEnemy()   //敌人方位识别
        {
            var largestEnemy = new AForge.Point(0, 0);
            var min_distance = 9999F;
            Bitmap image = filterRed(capture_screen(871, 127, 101, 80));
            var blobs = extractBlobs(image);

            if (blobs.Length == 0) return largestEnemy;

            // 返回距离最近的敌人
            //foreach (var blob in blobs)
            //{
            //    if (getDistance(blob.CenterOfGravity) <= min_distance)
            //        closestEnemy = blob.CenterOfGravity;
            //}
            largestEnemy = blobs[0].CenterOfGravity;
            log(string.Format("最近多敌人所在的方向为{0}， 距离为{1}", getAngle(largestEnemy).ToString("0.00"), getDistance(largestEnemy).ToString("0.00")));
            return largestEnemy;
        }

        public Bitmap extract_number_image(Bitmap image)    //击杀数识别
        {
            //string img_name = String.Format("{0}-origins.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //image.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);
            // create filter
            LevelsLinear le_filter = new LevelsLinear();
            // set ranges
            le_filter.InRed = new IntRange(57, 255);
            le_filter.InGreen = new IntRange(166, 207);
            le_filter.InBlue = new IntRange(0, 172);
            // apply the filter
            le_filter.ApplyInPlace(image);

            //img_name = String.Format("{0}-levels.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //image.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);

            //灰度化
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(image);

            //img_name = String.Format("{0}-灰度.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //grayImage.Save(Path.Combine(img_path, img_name), ImageFormat.Jpeg);

            //// 对比度
            ContrastStretch con_filter = new ContrastStretch();
            // apply the filter
            con_filter.ApplyInPlace(grayImage);

            //img_name = String.Format("{0}-对比.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //grayImage.Save(Path.Combine(img_path, img_name), ImageFormat.Jpeg);

            // threshold
            Threshold bw_filter = new Threshold(67);
            // apply the filter
            bw_filter.ApplyInPlace(grayImage);

            //string img_name = String.Format("{0}-threshold.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //grayImage.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);

            // （某些）反向
            Invert invert_filter = new Invert();
            var invert_grayImage = invert_filter.Apply(grayImage);
            //img_name = String.Format("{0}-invert.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //invert_grayImage.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);

            return grayImage;
        }

        public Bitmap extract_sight(Bitmap image)   //准星提取
        {
            //string img_name = String.Format("{0}-origins.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //image.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);

            ColorFiltering co_filter = new ColorFiltering();
            // set color ranges to keep
            co_filter.Red = new IntRange(0, 200);
            co_filter.Green = new IntRange(145, 255);
            co_filter.Blue = new IntRange(0, 170);
            // apply the filter
            co_filter.ApplyInPlace(image);

            co_filter = new ColorFiltering();
            // set color ranges to keep
            co_filter.Red = new IntRange(0, 150);
            co_filter.Green = new IntRange(170, 255);
            co_filter.Blue = new IntRange(0, 100);
            // apply the filter
            co_filter.ApplyInPlace(image);

            //string img_name = String.Format("{0}-color.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //image.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);

            return image;
        }

        public static int shotEnoughTime = 0;
        bool isShotEnough()
        {
            var cur_image = extract_number_image(capture_screen(903, 322, 36, 36));
            var cur_image_ten = extract_number_image(capture_screen(871, 322, 36, 36));
            ExhaustiveTemplateMatching _tm = new ExhaustiveTemplateMatching(0.9F);
            ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.72F);

            double max_result = 0;
            var max_i = -1;
            for (int i = 0; i < 10; i++)
            {
                double result = 0;
                var outcome = _tm.ProcessImage(cur_image, ChangePixelFormat(new Bitmap(Path.Combine(img_path, string.Format("{0}.jpeg", i))), PixelFormat.Format8bppIndexed));
                var outcome_in = tm.ProcessImage(cur_image, ChangePixelFormat(new Bitmap(Path.Combine(img_path, string.Format("{0}_invert.jpeg", i))), PixelFormat.Format8bppIndexed));


                if (outcome.Length > 0)
                {
                    result = outcome[0].Similarity;
                    //log("{0}的识别率", result);
                }

                if (max_result < result)
                {
                    max_result = result;
                    max_i = i;
                }

                if (outcome_in.Length > 0)
                {
                    result = outcome_in[0].Similarity;
                    //log("{0}的识别率", result);
                }
                if (max_result < result)
                {
                    max_result = result;
                    max_i = i;
                }
            }

            var ten_outcome = _tm.ProcessImage(cur_image_ten, ChangePixelFormat(new Bitmap(Path.Combine(img_path, string.Format("10.jpeg"))), PixelFormat.Format8bppIndexed));
            var ten_outcome_in = tm.ProcessImage(cur_image_ten, ChangePixelFormat(new Bitmap(Path.Combine(img_path, string.Format("10_invert.jpeg"))), PixelFormat.Format8bppIndexed));

            double ten_result = 0;

            if (ten_outcome.Length > 0)
            {
                ten_result = ten_outcome[0].Similarity;
                log(string.Format("已经打了10多个，识别率:{0}", ten_result.ToString("0.00")));
                shotEnoughTime++;
            }
            if (ten_outcome_in.Length > 0)
            {
                ten_result = ten_outcome_in[0].Similarity;
                log(string.Format("已经打了10多个,识别率:{0}", ten_result.ToString("0.00")));
                shotEnoughTime++;

            }
            if (max_result > 0)
            {
                log(string.Format("识别到已击杀数量为{0}，识别率为{1}", max_i, max_result.ToString("0.00")));
                if (max_i >= 4)
                    shotEnoughTime++;
            }
            if (shotEnoughTime >= 2)
            {
                log(string.Format("已识别到完成击杀次数{0}次", shotEnoughTime));
                return true;
            }
            return false;
        }

        public static void log(string msg)
        {
            //Console.WriteLine("[{0}] " + msg, DateTime.Now);
            Trace.WriteLine(string.Format("[{0}] " + msg, DateTime.Now));
        }

        public Thread t_shanqiang;
        public Thread t_attack;
        public Thread t_checkShotDown;
        public Thread t_spurt;
        public ExhaustiveTemplateMatching tm = new ExhaustiveTemplateMatching(0.85F);

        public void discover()
        {
            while (true)
            {
                this.ptrGame = FindWindow("_GONLINE_", null);
                SetForegroundWindow(ptrGame);
                SetWindowPos(ptrGame, 0, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

                if (isShotEnough())
                {
                    endDiscovering();
                    return;
                }
                else
                {
                    // 上一轮的探索战斗已结束
                    if (isShotDown)
                    {
                        log("新一轮探索...");
                        //全屏
                        //float left_rate = debug_match(624, 327, 48, 48, FLG_SIGHT_LEFT, 0.1F);
                        //float right_rate = debug_match(717, 327, 48, 48, FLG_SIGHT_RIGHT, 0.1F);
                        //active_auto_sight();
                        //if (isFirstEnemy || isExistEnemy()) //攻击
                            if (isFirstEnemy || isScanEnemy()) //攻击
                            {
                            isFirstEnemy = false;
                            isShotDown = false;
                            log("检测到敌军，直接攻击");
                            t_shanqiang = new Thread(shanQiang); // 闪枪线程
                            t_attack = new Thread(attack); // 攻击线程
                            //t_checkShotDown = new Thread(checkShotDown); // 判断敌方是否被击坠线程

                            t_shanqiang.Start();
                            t_attack.Start();
                            //t_checkShotDown.Start()


                            // 查看战斗是否应该结束

                            while (true)
                            {
                                if (isShotEnough())
                                {
                                    Mouse.ButtonUp(Mouse.MouseKeys.Right); //辅助瞄准取消
                                    endDiscovering();
                                    return;
                                }
                                Mouse.ButtonDown(Mouse.MouseKeys.Right); //辅助瞄准开始
                                if (!isExistEnemy() && notExistEnemyCount >= 4)
                                {
                                    isShotDown = true;
                                    notExistEnemyCount = 0;
                                    //log("已经击落，退出本轮探索");
                                    //if (t_attack != null)
                                    //    t_attack.Join();
                                    //if (t_shanqiang != null)
                                    //    t_shanqiang.Join();

                                    Mouse.ButtonUp(Mouse.MouseKeys.Right); //辅助瞄准取消
                                    break;
                                }
                                log("还未击落，继续本轮探索");
                            }
                            //if (t_attack != null)
                            //    t_attack.Join();
                            //if (t_shanqiang != null)
                            //    t_shanqiang.Join();
                            //if (t_checkShotDown != null)
                            //    t_checkShotDown.Join();
                        }
                        else //移动
                        {
                            // 未检测到敌军，移动
                            //var closestEnemy = getLargestEnemy();
                            //var angle = getAngle(closestEnemy);

                            //t_spurt = new Thread(spurt => getDistance(closestEnemy));
                            //t_spurt.Start();
                            //spurt(getDistance(closestEnemy));
                            //turn(getAngle(closestEnemy));
                            //spurt(8);
                            turn_left();

                            //Mouse.PressButton(Mouse.MouseKeys.Right, 400);
                            //turn_left();
                            //Mouse.PressButton(Mouse.MouseKeys.Right, 400);
                        }

                    }
                    log("此轮结束...");
                    //return;
                }
            }
        }

        //private System.Timers.Timer discoverTimer = new System.Timers.Timer(100);
        //private void discoverTimerEvent(Object source, ElapsedEventArgs e)  //探索
        //{
        //    if (isShotEnough())
        //    {
        //        discoverTimer.Enabled = false;
        //        endDiscovering();
        //        return;
        //    }
        //    else
        //    {
        //        if (isShotDown)
        //        {
        //            log("新一轮探索...");
        //            //全屏
        //            //float left_rate = debug_match(624, 327, 48, 48, FLG_SIGHT_LEFT, 0.1F);
        //            //float right_rate = debug_match(717, 327, 48, 48, FLG_SIGHT_RIGHT, 0.1F);

        //            if (isExistEnemy()) //攻击
        //            {
        //                //Mouse.ButtonDown(Mouse.MouseKeys.Right); //辅助瞄准
        //                isShotDown = false;
        //                log("检测到敌军，直接攻击");
        //                t_shanqiang = new Thread(shanQiang); // 闪枪线程
        //                t_attack = new Thread(attack); // 攻击线程
        //                t_checkShotDown = new Thread(checkShotDown); // 判断敌方是否被击坠线程

        //                t_shanqiang.Start();
        //                t_attack.Start();
        //                t_checkShotDown.Start();

        //                if (t_attack != null)
        //                    t_attack.Join();
        //                if (t_shanqiang != null)
        //                    t_shanqiang.Join();
        //                if (t_checkShotDown != null)
        //                    t_checkShotDown.Join();
        //            }
        //            else //移动
        //            {
        //                // 未检测到敌军，移动
        //                var closestEnemy = getLargestEnemy();
        //                var angle = getAngle(closestEnemy);

        //                turn(getAngle(closestEnemy));
        //                Mouse.ButtonDown(Mouse.MouseKeys.Right); //辅助瞄准帮助定位角度

        //                spurt(getDistance(closestEnemy));
        //                Mouse.ButtonUp(Mouse.MouseKeys.Right); //取消辅助瞄准
        //            }

        //        }
        //        log("此轮结束...");
        //        return;
        //    }

        //}

        public static int runtime = 0;
        public static int successtime = 0;
        public static bool nextMatchOK = true;

        private void kaGuaiOverTimerEvent(Object source, ElapsedEventArgs e) //卡怪结束
        {
            this.ptrGame = FindWindow("_GONLINE_", null);
            SetForegroundWindow(ptrGame);
            SetWindowPos(ptrGame, 0, 0, 0, 0, 0, SWP_NOZORDER | SWP_NOSIZE | SWP_SHOWWINDOW);

            mouse_click(515, 434, 800);//focus the window
            kaguaiTimer.Enabled = false;
            nextMatchOK = true;

            //等待结算界面

            Thread.Sleep(3000);
            log("还未进入结算页面");
            Thread.Sleep(500);
            Keyboard.KeyPress(Keys.Escape, 500);
            Thread.Sleep(500);
            Keyboard.KeyPress(Keys.Escape, 500);

            Mouse.PressButton(Mouse.MouseKeys.Right, 800);
            Thread.Sleep(500);
            Mouse.PressButton(Mouse.MouseKeys.Right, 800);

        }

        //******************** 图像处理 ********************
        ColorFiltering red_filter = new ColorFiltering();
        public Bitmap filterRed(Bitmap image) // color filter algorithm
        {
            // 提取红点
            //set color ranges to keep
            red_filter.Red = new IntRange(100, 255);
            red_filter.Green = new IntRange(0, 90);
            red_filter.Blue = new IntRange(0, 40);
            // apply the filter
            red_filter.ApplyInPlace(image);
            return image;
        }

        BlobCounterBase bc = new BlobCounter();
        public Blob[] extractBlobs(Bitmap image, int minWidth = 3, int minHeight = 3) // blob counter algorithm
        {
            // set filtering options
            bc.FilterBlobs = true;
            bc.MinWidth = minWidth;
            bc.MinHeight = minHeight;
            // set ordering options
            bc.ObjectsOrder = ObjectsOrder.Size;
            // process binary image
            bc.ProcessImage(image);
            return bc.GetObjectsInformation();
        }

        public Bitmap invert_image(Bitmap image)    // 图片反向
        {
            Invert invert_filter = new Invert();
            return invert_filter.Apply(image);
        }

        public bool match(int x, int y, int width, int height, string flag, float similarityThreshold)
        {
            Bitmap screenshot = capture_screen(x, y, width, height);

            var targetImage = new Bitmap(flag);
            var newTargetImage = ChangePixelFormat(new Bitmap(targetImage), PixelFormat.Format24bppRgb);
            // Setup the AForge library
            var tm = new ExhaustiveTemplateMatching(0.85F);
            // Process the images
            var results = tm.ProcessImage(screenshot, newTargetImage);
            // Compare the results, 0 indicates no match so return false
            if (results.Length > 0)
            {
                //log("confidence level" + results[0].Similarity);
                if (results[0].Similarity > similarityThreshold)
                    return true;
            }
            return false;
        }

        public float debug_match(int x, int y, int width, int height, string flag, float similarityThreshold)
        {
            Bitmap screenshot = capture_screen(x, y, width, height);
            //灰度化
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(screenshot);


            var targetImage = new Bitmap(flag);
            var newTargetImage = ChangePixelFormat(new Bitmap(targetImage), PixelFormat.Format8bppIndexed);
            // Setup the AForge library
            // Process the images
            var results = tm.ProcessImage(grayImage, newTargetImage);
            // Compare the results, 0 indicates no match so return false
            if (results.Length > 0)
            {
                //log("confidence level" + results[0].Similarity);
                return results[0].Similarity;
            }
            return 0;
        }

        public float fcolor_match(int x, int y, int width, int height, string flag, float similarityThreshold)
        {
            Bitmap screenshot = capture_screen(x, y, width, height);

            var targetImage = new Bitmap(flag);
            var newTargetImage = ChangePixelFormat(new Bitmap(targetImage), PixelFormat.Format24bppRgb);
            // Setup the AForge library
            // Process the images
            var results = tm.ProcessImage(screenshot, newTargetImage);
            // Compare the results, 0 indicates no match so return false
            if (results.Length > 0)
            {
                //log("confidence level" + results[0].Similarity);
                return results[0].Similarity;
            }
            return 0;
        }

        public float grey_match(int x, int y, int width, int height, string flag, float similarityThreshold)
        {
            Bitmap screenshot = capture_screen(x, y, width, height);
            //灰度化
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(screenshot);

            var targetImage = new Bitmap(flag);
            var newTargetImage = filter.Apply(targetImage);
            //var newTargetImage = ChangePixelFormat(new Bitmap(targetImage), PixelFormat.Format8bppIndexed);

            //string img_name1 = String.Format("cap-{0}.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //string img_name2 = String.Format("tar-{0}.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            //grayImage.Save(Path.Combine(img_path, img_name1), ImageFormat.Jpeg);
            //newTargetImage.Save(Path.Combine(img_path, img_name2), ImageFormat.Jpeg);

            // Setup the AForge library
            var tm = new ExhaustiveTemplateMatching(similarityThreshold);
            // Process the images
            var results = tm.ProcessImage(grayImage, newTargetImage);
            // Compare the results, 0 indicates no match so return false
            if (results.Length > 0)
            {
                //log("confidence level" + results[0].Similarity);
                return results[0].Similarity;
            }
            return 0;
        }

        private static Bitmap ChangePixelFormat(Bitmap inputImage, System.Drawing.Imaging.PixelFormat newFormat)
        {
            return (inputImage.Clone(new Rectangle(0, 0, inputImage.Width, inputImage.Height), newFormat));
        }

        // 从x,y开始的大小为width x height的截图,无需做分辨率转换
        public Bitmap capture_screen(int x, int y, int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域   
            imgGraphics.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            string img_name = String.Format("{0}.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            string tmp = Path.Combine(img_path, img_name);
            return image;
        }

        public Bitmap capture_save_screen(int x, int y, int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域   
            imgGraphics.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            string img_name = String.Format("{0}.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));
            image.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);
            return image;
        }

        public Bitmap capture_grey_save_screen(int x, int y, int width, int height)
        {
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            Graphics imgGraphics = Graphics.FromImage(image);
            //设置截屏区域   
            imgGraphics.CopyFromScreen(x, y, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);
            string img_name = String.Format("{0}.jpeg", DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss"));

            //灰度化
            Grayscale filter = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = filter.Apply(image);

            grayImage.Save(Path.Combine(img_path, img_name), System.Drawing.Imaging.ImageFormat.Jpeg);
            return grayImage;
        }

        public int dx(int x)
        {
            return x * 65535 / Screen.PrimaryScreen.Bounds.Width;
        }

        public int dy(int y)
        {
            return y * 65535 / Screen.PrimaryScreen.Bounds.Height;
        }

        //******************** 输入工具 ********************
        public void mouse_click(int x, int y, int delay = 500)
        {
            Mouse.Move(x, y);
            Thread.Sleep(delay);
            Mouse.PressButton(Mouse.MouseKeys.Left, delay);
        }

        public void mouse_click(MousePoint point, int delay = 500)
        {
            Mouse.Move(point.X, point.Y);
            Thread.Sleep(delay);
            Mouse.PressButton(Mouse.MouseKeys.Left, delay);

        }

        public void mouse_dclick(int x, int y, int delay = 500)
        {
            Mouse.Move(x, y);
            Thread.Sleep(800);
            Mouse.ButtonDown(Mouse.MouseKeys.Left);
            Thread.Sleep(delay);
            Mouse.ButtonUp(Mouse.MouseKeys.Left);
            Thread.Sleep(800);
            Mouse.ButtonDown(Mouse.MouseKeys.Left);
            Thread.Sleep(delay);
            Mouse.ButtonUp(Mouse.MouseKeys.Left);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X; public int Y; public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        //结构体布局 本机位置
        [StructLayout(LayoutKind.Sequential)]
        struct NativeRECT { public int left; public int top; public int right; public int bottom; }

        //********************************************* 变量标识 *******************************************



        bool isLoginLoaded()
        {
            return match(378, 425, 273, 54, FLG_LOGINLOAD, 0.95F);
        }

        bool isInputPass()
        {
            log("正在检测是否输入密码");
            return match(427, 368, 197, 19, FLG_PASS, 0.95F);
        }

        bool isPopLoginWindow()
        {
            if (match(504, 436, 28, 18, FLG_POPWIN, 0.90F))
            {
                log("检测到弹出窗口");
                mouse_click(516, 445);
                return true;
            }
            return false;

        }

        bool isLoginSuccess1()
        {
            return match(423, 331, 259, 61, FLG_LOGINSUCCESS1, 0.95F);
        }

        bool isLoginSuccess2()
        {
            return match(346, 321, 268, 101, FLG_LOGINSUCCESS2, 0.95F);
        }

        bool isEnterMissionServer()
        {
            return match(428, 97, 139, 30, FLG_ENTERMISSIONSER, 0.95F);
        }

        bool isCreateRoom()
        {
            return match(335, 213, 420, 44, FLG_CREATEROOM, 0.9F);
        }

        bool isMission()
        {
            return match(382, 311, 123, 65, FLG_MISSION, 0.95F);
        }

        bool isEnterRoom()
        {
            //&& match(832, 25, 194, 38, FLG_ENTERROOM2, 0.95F);
            return match(591, 660, 69, 48, FLG_ENTERROOM, 0.92F);
        }

        bool isBulletEmpty()
        {
            //return !match(605, 674, 34, 14, FLG_BULLETEMPTY, 0.95F); //全屏
            //return !match(438, 693, 33, 12, FLG_BULLETEMPTY, 0.95F); //窗口
            return !match(304, 692, 34, 15, FLG_BULLETEMPTY, 0.90F); //窗口
        }

        bool isMatchStart()
        {
            var result_1 = grey_match(20, 43, 40, 29, FLG_MATCHSTART, 0.8F);
            var result_2 = grey_match(908, 66, 31, 48, FLG_MATCHSTART_, 0.8F);
            //log(string.Format("任务开始识别一{0}，{1}", result_1, result_2));
            return result_1 > 0.8F || result_2 > 0.8F; //窗口
        }

        bool isMatchStart2()
        {
            var result_2 = grey_match(908, 66, 31, 48, FLG_MATCHSTART_, 0.8F);
            return result_2 > 0.8F; //窗口
        }

        bool isMatchEnd()
        {
            return match(568, 57, 159, 24, FLG_MATCHEND, 0.85F);
        }

        bool isStayTooClose()
        {
            return match(913, 163, 15, 15, FLG_STAY_CLOSE, 0.85F);
        }

        bool isOtherInRoom()
        {
            // 查看是否有人进房
            mouse_click(107, 300); //点击队员列表
            Thread.Sleep(1000);
            if (match(270, 338, 38, 11, FLG_OTHERINROOM1, 0.88F))
                return !(match(17, 354, 17, 17, FLG_OTHERINROOM2, 0.9F));
            return false;
        }

        public static bool isShotDown = false;
        public void checkShotDown()
        {
            //log("checkShotDown 开始");
            Mouse.ButtonUp(Mouse.MouseKeys.Right); //辅助瞄准开始
            while (true)
            {
                Thread.Sleep(3000);
                if (!isExistEnemy())
                {
                    isShotDown = true;
                    log("已经击落，退出本轮探索");
                    if (t_attack != null)
                        t_attack.Join();
                    if (t_shanqiang != null)
                        t_shanqiang.Join();

                    Mouse.ButtonUp(Mouse.MouseKeys.Right); //辅助瞄准取消
                    //log("checkShotDown 结束");
                    return;
                }
                log("还未击落，继续本轮探索");
            }

        }

        public void endDiscovering()
        {
            log(string.Format("已击杀数量{0}", shotEnoughTime));
            if (shotEnoughTime >= 1) //检测到3次已击杀足够
            {
                //discoverTimer.Enabled = false;
                isShotDown = true;
                //if (t_shanqiang != null)
                //    t_shanqiang.Abort();
                //if (t_attack != null)
                //    t_attack.Abort();
                //if (t_checkShotDown != null)
                //    t_checkShotDown.Abort();

                log("已击杀足够数量，停止本局探索");
                //Mouse.ButtonUp(Mouse.MouseKeys.Left);
                //Mouse.ButtonUp(Mouse.MouseKeys.Right);                    
                if (t_attack != null)
                    t_attack.Join();
                if (t_shanqiang != null)
                    t_shanqiang.Join();
                //if (t_checkShotDown != null)
                //    t_checkShotDown.Join();
                //Thread.Sleep(1500);
                ka_guai(); //开始卡怪
            }
        }






        public void ka_guai()
        {
            Keyboard.KeyUp(Keys.W);
            Keyboard.KeyUp(Keys.A);
            Keyboard.KeyUp(Keys.S);
            Keyboard.KeyUp(Keys.D);
            Thread.Sleep(900);
            log("开始卡怪");
            Keyboard.KeyPress(Keys.Enter, 800);
            Thread.Sleep(1000);
            Keyboard.KeyDown(Keys.LControlKey);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.LShiftKey);
            Thread.Sleep(500);
            Keyboard.KeyDown(Keys.M);
            Thread.Sleep(500);
            Keyboard.KeyUp(Keys.LControlKey);
            Keyboard.KeyUp(Keys.LShiftKey);
            Keyboard.KeyUp(Keys.M);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        [DllImport("User32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string strClass, string strWindow);

        //该函数获取一个窗口句柄,该窗口类名和窗口名与给定字符串匹配 hwnParent=Null从桌面窗口查找
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter,
            string strClass, string strWindow);

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(HandleRef hwnd, out NativeRECT rect);

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_SHOWWINDOW = 0x0040;
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);


        //定义变量
        private IntPtr ptrGame;
        private IntPtr ptrConsole;
        private const string ACCOUNT = "18383839745";
        private const string PASSWORD = "135289626480";
        private const string img_path = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp";
        private const string FLG_LOGINLOAD = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_login.jpeg";
        private const string FLG_PASS = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_password.jpeg";
        private const string FLG_POPWIN = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_pop_win.jpeg";

        private const string FLG_LOGINSUCCESS1 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_login_success_1.jpeg";
        private const string FLG_LOGINSUCCESS2 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_login_success_2.jpeg";
        private const string FLG_ENTERMISSIONSER = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_enter_mission.jpeg";
        private const string FLG_CREATEROOM = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_create_room.jpeg";
        private const string FLG_MISSION = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_mission.jpeg";
        private const string FLG_ENTERROOM = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_enter_room1.jpeg";
        //private const string FLG_ENTERROOM2 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_enter_room2.jpeg";

        private const string FLG_SIGHT = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_sight.jpeg";
        private const string FLG_SIGHT_LEFT = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_sight_left.jpeg";
        private const string FLG_SIGHT_RIGHT = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_sight_right.jpeg";

        private const string FLG_STAY_CLOSE = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_stay_too_close.jpeg";

        private const string FLG_BULLETEMPTY = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_bullet_empty.jpeg";
        private const string FLG_MATCHSTART = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_match_start.jpeg";
        private const string FLG_MATCHSTART_ = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_match_start_.jpeg";
        private const string FLG_MATCHEND = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_match_end.jpeg";


        private const string FLG_SHOTENOUGH1 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_shot_enough_1.jpeg";
        private const string FLG_SHOTENOUGH2 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_shot_enough_2.jpeg";
        private const string FLG_SHOTENOUGH3 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_shot_enough_3.jpeg";

        private const string FLG_OTHERINROOM1 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_other_in_room_1.jpeg";
        private const string FLG_OTHERINROOM2 = "E:\\Code\\C#\\SDGundamScript\\SDGundamScript\\tmp\\flag_other_in_room_2.jpeg";


        private double getDistance(AForge.Point p)
        {
            var A = p;
            var B = new IntPoint(51, 47);
            float x = B.X - A.X;
            float y = B.Y - A.Y;
            return Math.Sqrt(x * x + y * y);
        }

        private double getAngle(AForge.Point p)
        {
            var my_position = new IntPoint(51, 47);
            var enemy_position = p;
            return (Math.Atan2((my_position.Y - enemy_position.Y), (my_position.X - enemy_position.X)) * 180 / Math.PI) - 90;
        }
    }

}
