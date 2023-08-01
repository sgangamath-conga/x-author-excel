using Apttus.XAuthor.AppRuntime;
namespace Apttus.XAuthor.AppRuntime
{
    partial class RuntimeRibbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RuntimeRibbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();

            // MenuBuilder Builder = new MenuBuilder(this.tabAppBuilderRuntime);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher ribbonDialogLauncherImpl1 = this.Factory.CreateRibbonDialogLauncher();
            this.tabAppBuilderRuntime = this.Factory.CreateRibbonTab();
            this.ApplicationGroup = this.Factory.CreateRibbonGroup();
            this.ABGroup1 = this.Factory.CreateRibbonGroup();
            this.ABGroup2 = this.Factory.CreateRibbonGroup();
            this.ABGroup3 = this.Factory.CreateRibbonGroup();
            this.ABGroup4 = this.Factory.CreateRibbonGroup();
            this.ABGroup5 = this.Factory.CreateRibbonGroup();
            this.ABGroup6 = this.Factory.CreateRibbonGroup();
            this.ABGroup7 = this.Factory.CreateRibbonGroup();
            this.ABGroup8 = this.Factory.CreateRibbonGroup();
            this.ABGroup10 = this.Factory.CreateRibbonGroup();
            this.ABGroup9 = this.Factory.CreateRibbonGroup();
            this.EditGroup = this.Factory.CreateRibbonGroup();
            this.SocialGroup = this.Factory.CreateRibbonGroup();
            this.AccessGroup = this.Factory.CreateRibbonGroup();
            this.separator8 = this.Factory.CreateRibbonSeparator();
            this.apttusNotification = new System.Windows.Forms.NotifyIcon(this.components);
            this.mnuApplication = this.Factory.CreateRibbonMenu();
            this.button1 = this.Factory.CreateRibbonButton();
            this.button2 = this.Factory.CreateRibbonButton();
            this.button4 = this.Factory.CreateRibbonButton();
            this.button3 = this.Factory.CreateRibbonButton();
            this.button5 = this.Factory.CreateRibbonButton();
            this.button7 = this.Factory.CreateRibbonButton();
            this.button6 = this.Factory.CreateRibbonButton();
            this.button8 = this.Factory.CreateRibbonButton();
            this.button9 = this.Factory.CreateRibbonButton();
            this.button10 = this.Factory.CreateRibbonButton();
            this.button11 = this.Factory.CreateRibbonButton();
            this.button12 = this.Factory.CreateRibbonButton();
            this.button13 = this.Factory.CreateRibbonButton();
            this.button14 = this.Factory.CreateRibbonButton();
            this.button15 = this.Factory.CreateRibbonButton();
            this.button16 = this.Factory.CreateRibbonButton();
            this.button17 = this.Factory.CreateRibbonButton();
            this.button18 = this.Factory.CreateRibbonButton();
            this.button19 = this.Factory.CreateRibbonButton();
            this.button20 = this.Factory.CreateRibbonButton();
            this.button21 = this.Factory.CreateRibbonButton();
            this.button22 = this.Factory.CreateRibbonButton();
            this.button23 = this.Factory.CreateRibbonButton();
            this.button24 = this.Factory.CreateRibbonButton();
            this.button25 = this.Factory.CreateRibbonButton();
            this.button26 = this.Factory.CreateRibbonButton();
            this.button27 = this.Factory.CreateRibbonButton();
            this.button28 = this.Factory.CreateRibbonButton();
            this.button29 = this.Factory.CreateRibbonButton();
            this.button30 = this.Factory.CreateRibbonButton();
            this.button31 = this.Factory.CreateRibbonButton();
            this.button32 = this.Factory.CreateRibbonButton();
            this.button38 = this.Factory.CreateRibbonButton();
            this.button33 = this.Factory.CreateRibbonButton();
            this.button34 = this.Factory.CreateRibbonButton();
            this.button35 = this.Factory.CreateRibbonButton();
            this.button36 = this.Factory.CreateRibbonButton();
            this.button37 = this.Factory.CreateRibbonButton();
            this.button40 = this.Factory.CreateRibbonButton();
            this.button39 = this.Factory.CreateRibbonButton();
            this.button41 = this.Factory.CreateRibbonButton();
            this.button42 = this.Factory.CreateRibbonButton();
            this.button43 = this.Factory.CreateRibbonButton();
            this.button44 = this.Factory.CreateRibbonButton();
            this.button45 = this.Factory.CreateRibbonButton();
            this.button46 = this.Factory.CreateRibbonButton();
            this.button47 = this.Factory.CreateRibbonButton();
            this.button48 = this.Factory.CreateRibbonButton();
            this.button49 = this.Factory.CreateRibbonButton();
            this.button50 = this.Factory.CreateRibbonButton();
            this.button51 = this.Factory.CreateRibbonButton();
            this.button52 = this.Factory.CreateRibbonButton();
            this.button53 = this.Factory.CreateRibbonButton();
            this.button54 = this.Factory.CreateRibbonButton();
            this.button55 = this.Factory.CreateRibbonButton();
            this.button56 = this.Factory.CreateRibbonButton();
            this.button57 = this.Factory.CreateRibbonButton();
            this.button58 = this.Factory.CreateRibbonButton();
            this.button59 = this.Factory.CreateRibbonButton();
            this.button60 = this.Factory.CreateRibbonButton();
            this.button61 = this.Factory.CreateRibbonButton();
            this.button62 = this.Factory.CreateRibbonButton();
            this.button63 = this.Factory.CreateRibbonButton();
            this.button64 = this.Factory.CreateRibbonButton();
            this.button65 = this.Factory.CreateRibbonButton();
            this.button67 = this.Factory.CreateRibbonButton();
            this.button66 = this.Factory.CreateRibbonButton();
            this.button68 = this.Factory.CreateRibbonButton();
            this.button100 = this.Factory.CreateRibbonButton();
            this.button72 = this.Factory.CreateRibbonButton();
            this.button69 = this.Factory.CreateRibbonButton();
            this.button70 = this.Factory.CreateRibbonButton();
            this.button79 = this.Factory.CreateRibbonButton();
            this.button76 = this.Factory.CreateRibbonButton();
            this.button71 = this.Factory.CreateRibbonButton();
            this.button73 = this.Factory.CreateRibbonButton();
            this.button74 = this.Factory.CreateRibbonButton();
            this.button78 = this.Factory.CreateRibbonButton();
            this.button75 = this.Factory.CreateRibbonButton();
            this.button77 = this.Factory.CreateRibbonButton();
            this.button90 = this.Factory.CreateRibbonButton();
            this.button91 = this.Factory.CreateRibbonButton();
            this.button92 = this.Factory.CreateRibbonButton();
            this.button93 = this.Factory.CreateRibbonButton();
            this.button94 = this.Factory.CreateRibbonButton();
            this.button95 = this.Factory.CreateRibbonButton();
            this.button96 = this.Factory.CreateRibbonButton();
            this.button98 = this.Factory.CreateRibbonButton();
            this.button97 = this.Factory.CreateRibbonButton();
            this.button99 = this.Factory.CreateRibbonButton();
            this.button80 = this.Factory.CreateRibbonButton();
            this.button81 = this.Factory.CreateRibbonButton();
            this.button82 = this.Factory.CreateRibbonButton();
            this.button83 = this.Factory.CreateRibbonButton();
            this.button84 = this.Factory.CreateRibbonButton();
            this.button89 = this.Factory.CreateRibbonButton();
            this.button85 = this.Factory.CreateRibbonButton();
            this.button86 = this.Factory.CreateRibbonButton();
            this.button87 = this.Factory.CreateRibbonButton();
            this.button88 = this.Factory.CreateRibbonButton();
            this.btnAddRow = this.Factory.CreateRibbonSplitButton();
            this.Add3Rows = this.Factory.CreateRibbonButton();
            this.Add5Rows = this.Factory.CreateRibbonButton();
            this.Add10Rows = this.Factory.CreateRibbonButton();
            this.AddCustomRows = this.Factory.CreateRibbonButton();
            this.btnRemoveRow = this.Factory.CreateRibbonButton();
            this.btnChatterFeed = this.Factory.CreateRibbonToggleButton();
            this.btnConnect = this.Factory.CreateRibbonButton();
            this.mnuSwitchConnection = this.Factory.CreateRibbonMenu();
            this.btnGallerySupport = this.Factory.CreateRibbonGallery();
            this.btnAbout = this.Factory.CreateRibbonButton();
            this.tabAppBuilderRuntime.SuspendLayout();
            this.ApplicationGroup.SuspendLayout();
            this.ABGroup1.SuspendLayout();
            this.ABGroup2.SuspendLayout();
            this.ABGroup3.SuspendLayout();
            this.ABGroup4.SuspendLayout();
            this.ABGroup5.SuspendLayout();
            this.ABGroup6.SuspendLayout();
            this.ABGroup7.SuspendLayout();
            this.ABGroup8.SuspendLayout();
            this.ABGroup10.SuspendLayout();
            this.ABGroup9.SuspendLayout();
            this.EditGroup.SuspendLayout();
            this.SocialGroup.SuspendLayout();
            this.AccessGroup.SuspendLayout();
            // 
            // tabAppBuilderRuntime
            // 
            this.tabAppBuilderRuntime.Groups.Add(this.ApplicationGroup);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup1);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup2);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup3);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup4);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup5);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup6);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup7);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup8);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup10);
            this.tabAppBuilderRuntime.Groups.Add(this.ABGroup9);
            this.tabAppBuilderRuntime.Groups.Add(this.EditGroup);
            this.tabAppBuilderRuntime.Groups.Add(this.SocialGroup);
            this.tabAppBuilderRuntime.Groups.Add(this.AccessGroup);
            this.tabAppBuilderRuntime.Label = "X-Author Apps";
            this.tabAppBuilderRuntime.Name = "tabAppBuilderRuntime";
            // 
            // ApplicationGroup
            // 
            this.ApplicationGroup.Items.Add(this.mnuApplication);
            this.ApplicationGroup.Label = "Start";
            this.ApplicationGroup.Name = "ApplicationGroup";
            // 
            // ABGroup1
            // 
            this.ABGroup1.Items.Add(this.button1);
            this.ABGroup1.Items.Add(this.button2);
            this.ABGroup1.Items.Add(this.button4);
            this.ABGroup1.Items.Add(this.button3);
            this.ABGroup1.Items.Add(this.button5);
            this.ABGroup1.Items.Add(this.button7);
            this.ABGroup1.Items.Add(this.button6);
            this.ABGroup1.Items.Add(this.button8);
            this.ABGroup1.Items.Add(this.button9);
            this.ABGroup1.Items.Add(this.button10);
            this.ABGroup1.Label = "group1";
            this.ABGroup1.Name = "ABGroup1";
            this.ABGroup1.Visible = false;
            // 
            // ABGroup2
            // 
            this.ABGroup2.Items.Add(this.button11);
            this.ABGroup2.Items.Add(this.button12);
            this.ABGroup2.Items.Add(this.button13);
            this.ABGroup2.Items.Add(this.button14);
            this.ABGroup2.Items.Add(this.button15);
            this.ABGroup2.Items.Add(this.button16);
            this.ABGroup2.Items.Add(this.button17);
            this.ABGroup2.Items.Add(this.button18);
            this.ABGroup2.Items.Add(this.button19);
            this.ABGroup2.Items.Add(this.button20);
            this.ABGroup2.Label = "group2";
            this.ABGroup2.Name = "ABGroup2";
            this.ABGroup2.Visible = false;
            // 
            // ABGroup3
            // 
            this.ABGroup3.Items.Add(this.button21);
            this.ABGroup3.Items.Add(this.button22);
            this.ABGroup3.Items.Add(this.button23);
            this.ABGroup3.Items.Add(this.button24);
            this.ABGroup3.Items.Add(this.button25);
            this.ABGroup3.Items.Add(this.button26);
            this.ABGroup3.Items.Add(this.button27);
            this.ABGroup3.Items.Add(this.button28);
            this.ABGroup3.Items.Add(this.button29);
            this.ABGroup3.Items.Add(this.button30);
            this.ABGroup3.Label = "group3";
            this.ABGroup3.Name = "ABGroup3";
            this.ABGroup3.Visible = false;
            // 
            // ABGroup4
            // 
            this.ABGroup4.Items.Add(this.button31);
            this.ABGroup4.Items.Add(this.button32);
            this.ABGroup4.Items.Add(this.button38);
            this.ABGroup4.Items.Add(this.button33);
            this.ABGroup4.Items.Add(this.button34);
            this.ABGroup4.Items.Add(this.button35);
            this.ABGroup4.Items.Add(this.button36);
            this.ABGroup4.Items.Add(this.button37);
            this.ABGroup4.Items.Add(this.button40);
            this.ABGroup4.Items.Add(this.button39);
            this.ABGroup4.Label = "group4";
            this.ABGroup4.Name = "ABGroup4";
            this.ABGroup4.Visible = false;
            // 
            // ABGroup5
            // 
            this.ABGroup5.Items.Add(this.button41);
            this.ABGroup5.Items.Add(this.button42);
            this.ABGroup5.Items.Add(this.button43);
            this.ABGroup5.Items.Add(this.button44);
            this.ABGroup5.Items.Add(this.button45);
            this.ABGroup5.Items.Add(this.button46);
            this.ABGroup5.Items.Add(this.button47);
            this.ABGroup5.Items.Add(this.button48);
            this.ABGroup5.Items.Add(this.button49);
            this.ABGroup5.Items.Add(this.button50);
            this.ABGroup5.Label = "group5";
            this.ABGroup5.Name = "ABGroup5";
            this.ABGroup5.Visible = false;
            // 
            // ABGroup6
            // 
            this.ABGroup6.Items.Add(this.button51);
            this.ABGroup6.Items.Add(this.button52);
            this.ABGroup6.Items.Add(this.button53);
            this.ABGroup6.Items.Add(this.button54);
            this.ABGroup6.Items.Add(this.button55);
            this.ABGroup6.Items.Add(this.button56);
            this.ABGroup6.Items.Add(this.button57);
            this.ABGroup6.Items.Add(this.button58);
            this.ABGroup6.Items.Add(this.button59);
            this.ABGroup6.Items.Add(this.button60);
            this.ABGroup6.Label = "group6";
            this.ABGroup6.Name = "ABGroup6";
            this.ABGroup6.Visible = false;
            // 
            // ABGroup7
            // 
            this.ABGroup7.Items.Add(this.button61);
            this.ABGroup7.Items.Add(this.button62);
            this.ABGroup7.Items.Add(this.button63);
            this.ABGroup7.Items.Add(this.button64);
            this.ABGroup7.Items.Add(this.button65);
            this.ABGroup7.Items.Add(this.button67);
            this.ABGroup7.Items.Add(this.button66);
            this.ABGroup7.Items.Add(this.button68);
            this.ABGroup7.Items.Add(this.button100);
            this.ABGroup7.Items.Add(this.button72);
            this.ABGroup7.Label = "group7";
            this.ABGroup7.Name = "ABGroup7";
            this.ABGroup7.Visible = false;
            // 
            // ABGroup8
            // 
            this.ABGroup8.Items.Add(this.button69);
            this.ABGroup8.Items.Add(this.button70);
            this.ABGroup8.Items.Add(this.button79);
            this.ABGroup8.Items.Add(this.button76);
            this.ABGroup8.Items.Add(this.button71);
            this.ABGroup8.Items.Add(this.button73);
            this.ABGroup8.Items.Add(this.button74);
            this.ABGroup8.Items.Add(this.button78);
            this.ABGroup8.Items.Add(this.button75);
            this.ABGroup8.Items.Add(this.button77);
            this.ABGroup8.Label = "group8";
            this.ABGroup8.Name = "ABGroup8";
            this.ABGroup8.Visible = false;
            // 
            // ABGroup10
            // 
            this.ABGroup10.Items.Add(this.button90);
            this.ABGroup10.Items.Add(this.button91);
            this.ABGroup10.Items.Add(this.button92);
            this.ABGroup10.Items.Add(this.button93);
            this.ABGroup10.Items.Add(this.button94);
            this.ABGroup10.Items.Add(this.button95);
            this.ABGroup10.Items.Add(this.button96);
            this.ABGroup10.Items.Add(this.button98);
            this.ABGroup10.Items.Add(this.button97);
            this.ABGroup10.Items.Add(this.button99);
            this.ABGroup10.Label = "group10";
            this.ABGroup10.Name = "ABGroup10";
            this.ABGroup10.Visible = false;
            // 
            // ABGroup9
            // 
            this.ABGroup9.Items.Add(this.button80);
            this.ABGroup9.Items.Add(this.button81);
            this.ABGroup9.Items.Add(this.button82);
            this.ABGroup9.Items.Add(this.button83);
            this.ABGroup9.Items.Add(this.button84);
            this.ABGroup9.Items.Add(this.button89);
            this.ABGroup9.Items.Add(this.button85);
            this.ABGroup9.Items.Add(this.button86);
            this.ABGroup9.Items.Add(this.button87);
            this.ABGroup9.Items.Add(this.button88);
            this.ABGroup9.Label = "group9";
            this.ABGroup9.Name = "ABGroup9";
            this.ABGroup9.Visible = false;
            // 
            // EditGroup
            // 
            this.EditGroup.Items.Add(this.btnAddRow);
            this.EditGroup.Items.Add(this.btnRemoveRow);
            this.EditGroup.Label = "Edit";
            this.EditGroup.Name = "EditGroup";
            this.EditGroup.Visible = false;
            // 
            // SocialGroup
            // 
            this.SocialGroup.Items.Add(this.btnChatterFeed);
            this.SocialGroup.Label = "Social";
            this.SocialGroup.Name = "SocialGroup";
            this.SocialGroup.Visible = false;
            // 
            // AccessGroup
            // 
            this.AccessGroup.DialogLauncher = ribbonDialogLauncherImpl1;
            this.AccessGroup.Items.Add(this.btnConnect);
            this.AccessGroup.Items.Add(this.mnuSwitchConnection);
            this.AccessGroup.Items.Add(this.separator8);
            this.AccessGroup.Items.Add(this.btnGallerySupport);
            this.AccessGroup.Label = "Access";
            this.AccessGroup.Name = "AccessGroup";
            this.AccessGroup.DialogLauncherClick += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AccessGroup_DialogLauncherClick);
            // 
            // separator8
            // 
            this.separator8.Name = "separator8";
            // 
            // apttusNotification
            // 
            this.apttusNotification.Visible = true;
            // 
            // mnuApplication
            // 
            this.mnuApplication.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.mnuApplication.Dynamic = true;
            this.mnuApplication.Label = "Apps";
            this.mnuApplication.Name = "mnuApplication";
            this.mnuApplication.OfficeImageId = "AnimationPreview";
            this.mnuApplication.ShowImage = true;
            // 
            // button1
            // 
            this.button1.Label = "button1";
            this.button1.Name = "button1";
            this.button1.Visible = false;
            // 
            // button2
            // 
            this.button2.Label = "button2";
            this.button2.Name = "button2";
            this.button2.Visible = false;
            // 
            // button4
            // 
            this.button4.Label = "button4";
            this.button4.Name = "button4";
            this.button4.Visible = false;
            // 
            // button3
            // 
            this.button3.Label = "button3";
            this.button3.Name = "button3";
            this.button3.Visible = false;
            // 
            // button5
            // 
            this.button5.Label = "button5";
            this.button5.Name = "button5";
            this.button5.Visible = false;
            // 
            // button7
            // 
            this.button7.Label = "button7";
            this.button7.Name = "button7";
            this.button7.Visible = false;
            // 
            // button6
            // 
            this.button6.Label = "button6";
            this.button6.Name = "button6";
            this.button6.Visible = false;
            // 
            // button8
            // 
            this.button8.Label = "button8";
            this.button8.Name = "button8";
            this.button8.Visible = false;
            // 
            // button9
            // 
            this.button9.Label = "button9";
            this.button9.Name = "button9";
            this.button9.Visible = false;
            // 
            // button10
            // 
            this.button10.Label = "button10";
            this.button10.Name = "button10";
            this.button10.Visible = false;
            // 
            // button11
            // 
            this.button11.Label = "button11";
            this.button11.Name = "button11";
            this.button11.Visible = false;
            // 
            // button12
            // 
            this.button12.Label = "button12";
            this.button12.Name = "button12";
            this.button12.Visible = false;
            // 
            // button13
            // 
            this.button13.Label = "button13";
            this.button13.Name = "button13";
            this.button13.Visible = false;
            // 
            // button14
            // 
            this.button14.Label = "button14";
            this.button14.Name = "button14";
            this.button14.Visible = false;
            // 
            // button15
            // 
            this.button15.Label = "button15";
            this.button15.Name = "button15";
            this.button15.Visible = false;
            // 
            // button16
            // 
            this.button16.Label = "button16";
            this.button16.Name = "button16";
            this.button16.Visible = false;
            // 
            // button17
            // 
            this.button17.Label = "button17";
            this.button17.Name = "button17";
            this.button17.Visible = false;
            // 
            // button18
            // 
            this.button18.Label = "button18";
            this.button18.Name = "button18";
            this.button18.Visible = false;
            // 
            // button19
            // 
            this.button19.Label = "button19";
            this.button19.Name = "button19";
            this.button19.Visible = false;
            // 
            // button20
            // 
            this.button20.Label = "button20";
            this.button20.Name = "button20";
            this.button20.Visible = false;
            // 
            // button21
            // 
            this.button21.Label = "button21";
            this.button21.Name = "button21";
            this.button21.Visible = false;
            // 
            // button22
            // 
            this.button22.Label = "button22";
            this.button22.Name = "button22";
            this.button22.Visible = false;
            // 
            // button23
            // 
            this.button23.Label = "button23";
            this.button23.Name = "button23";
            this.button23.Visible = false;
            // 
            // button24
            // 
            this.button24.Label = "button24";
            this.button24.Name = "button24";
            this.button24.Visible = false;
            // 
            // button25
            // 
            this.button25.Label = "button25";
            this.button25.Name = "button25";
            this.button25.Visible = false;
            // 
            // button26
            // 
            this.button26.Label = "button26";
            this.button26.Name = "button26";
            this.button26.Visible = false;
            // 
            // button27
            // 
            this.button27.Label = "button27";
            this.button27.Name = "button27";
            this.button27.Visible = false;
            // 
            // button28
            // 
            this.button28.Label = "button28";
            this.button28.Name = "button28";
            this.button28.Visible = false;
            // 
            // button29
            // 
            this.button29.Label = "button29";
            this.button29.Name = "button29";
            this.button29.Visible = false;
            // 
            // button30
            // 
            this.button30.Label = "button30";
            this.button30.Name = "button30";
            this.button30.Visible = false;
            // 
            // button31
            // 
            this.button31.Label = "button31";
            this.button31.Name = "button31";
            this.button31.Visible = false;
            // 
            // button32
            // 
            this.button32.Label = "button32";
            this.button32.Name = "button32";
            this.button32.Visible = false;
            // 
            // button38
            // 
            this.button38.Label = "button38";
            this.button38.Name = "button38";
            this.button38.Visible = false;
            // 
            // button33
            // 
            this.button33.Label = "button33";
            this.button33.Name = "button33";
            this.button33.Visible = false;
            // 
            // button34
            // 
            this.button34.Label = "button34";
            this.button34.Name = "button34";
            this.button34.Visible = false;
            // 
            // button35
            // 
            this.button35.Label = "button35";
            this.button35.Name = "button35";
            this.button35.Visible = false;
            // 
            // button36
            // 
            this.button36.Label = "button36";
            this.button36.Name = "button36";
            this.button36.Visible = false;
            // 
            // button37
            // 
            this.button37.Label = "button37";
            this.button37.Name = "button37";
            this.button37.Visible = false;
            // 
            // button40
            // 
            this.button40.Label = "button40";
            this.button40.Name = "button40";
            this.button40.Visible = false;
            // 
            // button39
            // 
            this.button39.Label = "button39";
            this.button39.Name = "button39";
            this.button39.Visible = false;
            // 
            // button41
            // 
            this.button41.Label = "button41";
            this.button41.Name = "button41";
            this.button41.Visible = false;
            // 
            // button42
            // 
            this.button42.Label = "button42";
            this.button42.Name = "button42";
            this.button42.Visible = false;
            // 
            // button43
            // 
            this.button43.Label = "button43";
            this.button43.Name = "button43";
            this.button43.Visible = false;
            // 
            // button44
            // 
            this.button44.Label = "button44";
            this.button44.Name = "button44";
            this.button44.Visible = false;
            // 
            // button45
            // 
            this.button45.Label = "button45";
            this.button45.Name = "button45";
            this.button45.Visible = false;
            // 
            // button46
            // 
            this.button46.Label = "button46";
            this.button46.Name = "button46";
            this.button46.Visible = false;
            // 
            // button47
            // 
            this.button47.Label = "button47";
            this.button47.Name = "button47";
            this.button47.Visible = false;
            // 
            // button48
            // 
            this.button48.Label = "button48";
            this.button48.Name = "button48";
            this.button48.Visible = false;
            // 
            // button49
            // 
            this.button49.Label = "button49";
            this.button49.Name = "button49";
            this.button49.Visible = false;
            // 
            // button50
            // 
            this.button50.Label = "button50";
            this.button50.Name = "button50";
            this.button50.Visible = false;
            // 
            // button51
            // 
            this.button51.Label = "button51";
            this.button51.Name = "button51";
            this.button51.Visible = false;
            // 
            // button52
            // 
            this.button52.Label = "button52";
            this.button52.Name = "button52";
            this.button52.Visible = false;
            // 
            // button53
            // 
            this.button53.Label = "button53";
            this.button53.Name = "button53";
            this.button53.Visible = false;
            // 
            // button54
            // 
            this.button54.Label = "button54";
            this.button54.Name = "button54";
            this.button54.Visible = false;
            // 
            // button55
            // 
            this.button55.Label = "button55";
            this.button55.Name = "button55";
            this.button55.Visible = false;
            // 
            // button56
            // 
            this.button56.Label = "button56";
            this.button56.Name = "button56";
            this.button56.Visible = false;
            // 
            // button57
            // 
            this.button57.Label = "button57";
            this.button57.Name = "button57";
            this.button57.Visible = false;
            // 
            // button58
            // 
            this.button58.Label = "button58";
            this.button58.Name = "button58";
            this.button58.Visible = false;
            // 
            // button59
            // 
            this.button59.Label = "button59";
            this.button59.Name = "button59";
            this.button59.Visible = false;
            // 
            // button60
            // 
            this.button60.Label = "button60";
            this.button60.Name = "button60";
            this.button60.Visible = false;
            // 
            // button61
            // 
            this.button61.Label = "button61";
            this.button61.Name = "button61";
            this.button61.Visible = false;
            // 
            // button62
            // 
            this.button62.Label = "button62";
            this.button62.Name = "button62";
            this.button62.Visible = false;
            // 
            // button63
            // 
            this.button63.Label = "button63";
            this.button63.Name = "button63";
            this.button63.Visible = false;
            // 
            // button64
            // 
            this.button64.Label = "button64";
            this.button64.Name = "button64";
            this.button64.Visible = false;
            // 
            // button65
            // 
            this.button65.Label = "button65";
            this.button65.Name = "button65";
            this.button65.Visible = false;
            // 
            // button67
            // 
            this.button67.Label = "button67";
            this.button67.Name = "button67";
            this.button67.Visible = false;
            // 
            // button66
            // 
            this.button66.Label = "button66";
            this.button66.Name = "button66";
            this.button66.Visible = false;
            // 
            // button68
            // 
            this.button68.Label = "button68";
            this.button68.Name = "button68";
            this.button68.Visible = false;
            // 
            // button100
            // 
            this.button100.Label = "button100";
            this.button100.Name = "button100";
            this.button100.Visible = false;
            // 
            // button72
            // 
            this.button72.Label = "button72";
            this.button72.Name = "button72";
            this.button72.Visible = false;
            // 
            // button69
            // 
            this.button69.Label = "button69";
            this.button69.Name = "button69";
            this.button69.Visible = false;
            // 
            // button70
            // 
            this.button70.Label = "button70";
            this.button70.Name = "button70";
            this.button70.Visible = false;
            // 
            // button79
            // 
            this.button79.Label = "button79";
            this.button79.Name = "button79";
            this.button79.Visible = false;
            // 
            // button76
            // 
            this.button76.Label = "button76";
            this.button76.Name = "button76";
            this.button76.Visible = false;
            // 
            // button71
            // 
            this.button71.Label = "button71";
            this.button71.Name = "button71";
            this.button71.Visible = false;
            // 
            // button73
            // 
            this.button73.Label = "button73";
            this.button73.Name = "button73";
            this.button73.Visible = false;
            // 
            // button74
            // 
            this.button74.Label = "button74";
            this.button74.Name = "button74";
            this.button74.Visible = false;
            // 
            // button78
            // 
            this.button78.Label = "button78";
            this.button78.Name = "button78";
            this.button78.Visible = false;
            // 
            // button75
            // 
            this.button75.Label = "button75";
            this.button75.Name = "button75";
            this.button75.Visible = false;
            // 
            // button77
            // 
            this.button77.Label = "button77";
            this.button77.Name = "button77";
            this.button77.Visible = false;
            // 
            // button90
            // 
            this.button90.Label = "button90";
            this.button90.Name = "button90";
            this.button90.Visible = false;
            // 
            // button91
            // 
            this.button91.Label = "button91";
            this.button91.Name = "button91";
            this.button91.Visible = false;
            // 
            // button92
            // 
            this.button92.Label = "button92";
            this.button92.Name = "button92";
            this.button92.Visible = false;
            // 
            // button93
            // 
            this.button93.Label = "button93";
            this.button93.Name = "button93";
            this.button93.Visible = false;
            // 
            // button94
            // 
            this.button94.Label = "button94";
            this.button94.Name = "button94";
            this.button94.Visible = false;
            // 
            // button95
            // 
            this.button95.Label = "button95";
            this.button95.Name = "button95";
            this.button95.Visible = false;
            // 
            // button96
            // 
            this.button96.Label = "button96";
            this.button96.Name = "button96";
            this.button96.Visible = false;
            // 
            // button98
            // 
            this.button98.Label = "button98";
            this.button98.Name = "button98";
            this.button98.Visible = false;
            // 
            // button97
            // 
            this.button97.Label = "button97";
            this.button97.Name = "button97";
            this.button97.Visible = false;
            // 
            // button99
            // 
            this.button99.Label = "button99";
            this.button99.Name = "button99";
            this.button99.Visible = false;
            // 
            // button80
            // 
            this.button80.Label = "button80";
            this.button80.Name = "button80";
            this.button80.Visible = false;
            // 
            // button81
            // 
            this.button81.Label = "button81";
            this.button81.Name = "button81";
            this.button81.Visible = false;
            // 
            // button82
            // 
            this.button82.Label = "button82";
            this.button82.Name = "button82";
            this.button82.Visible = false;
            // 
            // button83
            // 
            this.button83.Label = "button83";
            this.button83.Name = "button83";
            this.button83.Visible = false;
            // 
            // button84
            // 
            this.button84.Label = "button84";
            this.button84.Name = "button84";
            this.button84.Visible = false;
            // 
            // button89
            // 
            this.button89.Label = "button89";
            this.button89.Name = "button89";
            this.button89.Visible = false;
            // 
            // button85
            // 
            this.button85.Label = "button85";
            this.button85.Name = "button85";
            this.button85.Visible = false;
            // 
            // button86
            // 
            this.button86.Label = "button86";
            this.button86.Name = "button86";
            this.button86.Visible = false;
            // 
            // button87
            // 
            this.button87.Label = "button87";
            this.button87.Name = "button87";
            this.button87.Visible = false;
            // 
            // button88
            // 
            this.button88.Label = "button88";
            this.button88.Name = "button88";
            this.button88.Visible = false;
            // 
            // btnAddRow
            // 
            this.btnAddRow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnAddRow.Items.Add(this.Add3Rows);
            this.btnAddRow.Items.Add(this.Add5Rows);
            this.btnAddRow.Items.Add(this.Add10Rows);
            this.btnAddRow.Items.Add(this.AddCustomRows);
            this.btnAddRow.Label = "Add Row";
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.OfficeImageId = "InsertCopiedCells";
            this.btnAddRow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAddRow_Click);
            // 
            // Add3Rows
            // 
            this.Add3Rows.Label = "3 Rows";
            this.Add3Rows.Name = "Add3Rows";
            this.Add3Rows.ShowImage = true;
            this.Add3Rows.Tag = "3 Rows";
            this.Add3Rows.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Add3Rows_Click);
            // 
            // Add5Rows
            // 
            this.Add5Rows.Label = "5 Rows";
            this.Add5Rows.Name = "Add5Rows";
            this.Add5Rows.ShowImage = true;
            this.Add5Rows.Tag = "5 Rows";
            this.Add5Rows.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Add5Rows_Click);
            // 
            // Add10Rows
            // 
            this.Add10Rows.Label = "10 Rows";
            this.Add10Rows.Name = "Add10Rows";
            this.Add10Rows.ShowImage = true;
            this.Add10Rows.Tag = "10 Rows";
            this.Add10Rows.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.Add10Rows_Click);
            // 
            // AddCustomRows
            // 
            this.AddCustomRows.Label = "Custom Rows";
            this.AddCustomRows.Name = "AddCustomRows";
            this.AddCustomRows.ShowImage = true;
            this.AddCustomRows.Tag = "Custom Rows";
            this.AddCustomRows.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AddCustomRows_Click);
            // 
            // btnRemoveRow
            // 
            this.btnRemoveRow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnRemoveRow.Label = "Delete Row(s) from Salesforce";
            this.btnRemoveRow.Name = "btnRemoveRow";
            this.btnRemoveRow.OfficeImageId = "DeleteCells";
            this.btnRemoveRow.ShowImage = true;
            this.btnRemoveRow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnRemoveRow_Click);
            // 
            // btnChatterFeed
            // 
            this.btnChatterFeed.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnChatterFeed.Label = "Chatter";
            this.btnChatterFeed.Name = "btnChatterFeed";
            this.btnChatterFeed.OfficeImageId = "NameManager";
            this.btnChatterFeed.ShowImage = true;
            this.btnChatterFeed.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnChatterFeed_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnConnect.Label = "Connect";
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.OfficeImageId = "DatabasePermissionsMenu";
            this.btnConnect.ShowImage = true;
            this.btnConnect.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnConnect_Click);
            // 
            // mnuSwitchConnection
            // 
            this.mnuSwitchConnection.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.mnuSwitchConnection.Dynamic = true;
            this.mnuSwitchConnection.Label = "Switch Connection";
            this.mnuSwitchConnection.Name = "mnuSwitchConnection";
            this.mnuSwitchConnection.OfficeImageId = "DirectRepliesTo";
            this.mnuSwitchConnection.ShowImage = true;
            this.mnuSwitchConnection.ItemsLoading += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.mnuSwitchConnection_ItemsLoading);
            // 
            // btnGallerySupport
            // 
            this.btnGallerySupport.Buttons.Add(this.btnAbout);
            this.btnGallerySupport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnGallerySupport.Label = "Support";
            this.btnGallerySupport.Name = "btnGallerySupport";
            this.btnGallerySupport.OfficeImageId = "TentativeAcceptInvitation";
            this.btnGallerySupport.ShowImage = true;
            // 
            // btnAbout
            // 
            this.btnAbout.Label = "About X-Author Excel...";
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.ShowImage = true;
            this.btnAbout.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAbout_Click);
            // 
            // RuntimeRibbon
            // 
            this.Name = "RuntimeRibbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabAppBuilderRuntime);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.RuntimeRibbon_Load);
            this.tabAppBuilderRuntime.ResumeLayout(false);
            this.tabAppBuilderRuntime.PerformLayout();
            this.ApplicationGroup.ResumeLayout(false);
            this.ApplicationGroup.PerformLayout();
            this.ABGroup1.ResumeLayout(false);
            this.ABGroup1.PerformLayout();
            this.ABGroup2.ResumeLayout(false);
            this.ABGroup2.PerformLayout();
            this.ABGroup3.ResumeLayout(false);
            this.ABGroup3.PerformLayout();
            this.ABGroup4.ResumeLayout(false);
            this.ABGroup4.PerformLayout();
            this.ABGroup5.ResumeLayout(false);
            this.ABGroup5.PerformLayout();
            this.ABGroup6.ResumeLayout(false);
            this.ABGroup6.PerformLayout();
            this.ABGroup7.ResumeLayout(false);
            this.ABGroup7.PerformLayout();
            this.ABGroup8.ResumeLayout(false);
            this.ABGroup8.PerformLayout();
            this.ABGroup10.ResumeLayout(false);
            this.ABGroup10.PerformLayout();
            this.ABGroup9.ResumeLayout(false);
            this.ABGroup9.PerformLayout();
            this.EditGroup.ResumeLayout(false);
            this.EditGroup.PerformLayout();
            this.SocialGroup.ResumeLayout(false);
            this.SocialGroup.PerformLayout();
            this.AccessGroup.ResumeLayout(false);
            this.AccessGroup.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabAppBuilderRuntime;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ApplicationGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu mnuApplication;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup AccessGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnConnect;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu mnuSwitchConnection;
        internal Microsoft.Office.Tools.Ribbon.RibbonSeparator separator8;
        internal Microsoft.Office.Tools.Ribbon.RibbonGallery btnGallerySupport;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnAbout;
        internal System.Windows.Forms.NotifyIcon apttusNotification;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button1;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button4;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button5;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button7;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button6;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button8;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button9;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button10;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup2;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button11;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button12;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button13;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button14;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button15;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button16;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button17;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button18;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button19;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button20;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup3;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button21;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button22;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button23;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button24;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button25;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button26;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button27;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button28;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button29;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button30;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup4;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button31;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button32;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button38;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button33;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button34;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button35;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button36;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button37;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button40;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button39;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup5;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button41;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button42;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button43;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button44;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button45;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button46;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button47;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button48;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button49;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button50;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup6;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button51;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button52;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button53;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button54;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button55;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button56;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button57;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button58;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button59;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button60;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup7;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button61;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button62;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button63;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button64;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button65;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button67;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button66;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button68;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button72;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup8;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button69;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button70;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button79;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button76;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button71;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button73;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button74;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button78;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button75;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button77;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup9;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button80;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button81;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button82;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button83;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button84;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button89;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button85;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button86;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button87;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button88;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button100;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup ABGroup10;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button90;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button91;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button92;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button93;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button94;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button95;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button96;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button98;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button97;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button99;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup EditGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton btnAddRow;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnRemoveRow;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Add3Rows;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Add5Rows;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton Add10Rows;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup SocialGroup;
        internal Microsoft.Office.Tools.Ribbon.RibbonToggleButton btnChatterFeed;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton AddCustomRows;
    }

    partial class ThisRibbonCollection
    {
        internal RuntimeRibbon RuntimeRibbon
        {
            get { return this.GetRibbon<RuntimeRibbon>(); }
        }
    }
}
