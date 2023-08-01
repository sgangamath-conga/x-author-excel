namespace Apttus.XAuthor.Core
{
    /*
     Font     Size     Text Color     Fill Color

     Title     Calibri     16
     List Header
     Display only fields
     Save Fields
     */
    public class QuickAppSettings
    {
        public QuickAppSettings()
        {
            AppTitle = new TitleAttribute();
            DisplayOnlyField = new DisplayOnlyFieldsAttribute();
            ListHeader = new ListHeaderAttribute();
            SaveOnlyField = new SaveOnlyFieldsAttribute();
            ListAppPosition = new Postion();
            ParentChildAppPosition = new Postion();



        }

        public TitleAttribute AppTitle
        {
            get;
            set;
        }
        public DisplayOnlyFieldsAttribute DisplayOnlyField
        {
            get;
            set;
        }
        public ListHeaderAttribute ListHeader
        {
            get;
            set;
        }
        public SaveOnlyFieldsAttribute SaveOnlyField
        {
            get;
            set;
        }


        public Postion ListAppPosition
        {
            get;
            set;
        }
        public Postion ParentChildAppPosition
        {
            get;
            set;
        }
    }

    public class FormatSetting
    {
        public FormatSetting()
        {
            // AppFontAndColor = new AppFontColor();

        }
        public AppFontColor AppFontAndColor
        {
            get;
            set;
        }
        /*
        public Font Font
        {
            get;
            set;
        }
        public Color TextColor
        {
            get;
            set;
        }
        public Color FillColor
        {
            get;
            set;
        }
         */
    }
    public abstract class AppAttributes
    {
        private const string DEFAULT_OTHER_FONT = "Calibri, 11.25pt";
        private const string DEFAULT_OTHER_FONT_COLOR = "-1";
        public FormatSetting FormatAttribute
        {
            get;
            set;
        }
        public void SetAppAttributes(AppFontColor fontnColor)
        {
            FormatAttribute.AppFontAndColor = fontnColor;

        }
        public AppAttributes()
        {
            FormatAttribute = new FormatSetting();
        }
        public string DefaultFontString()
        {
            return DEFAULT_OTHER_FONT;

        }
        public string DefaultFontColor()
        {
            return DEFAULT_OTHER_FONT_COLOR;
        }

    }
    public class TitleAttribute : AppAttributes
    {
        private const string DEFAULT_TITLE_FONT = "Calibri, 16.25pt";
        public TitleAttribute()
            : base()
        {
            AppFontColor fnt = new AppFontColor();
            fnt.FontStr = DEFAULT_TITLE_FONT;
            fnt.AppTextColor = "-16777216";
            fnt.AppFillColor = DefaultFontColor();
            SetAppAttributes(fnt);
        }
        public string GetDefaultFont()
        {
            return DEFAULT_TITLE_FONT;
        }
    }

    public class ListHeaderAttribute : AppAttributes
    {

    }

    public class DisplayOnlyFieldsAttribute : AppAttributes
    {
        private const string DEFAULT_FILL_COLOR = "-7155632";
        public DisplayOnlyFieldsAttribute()
            : base()
        {
            AppFontColor fnt = new AppFontColor();
            fnt.FontStr = DefaultFontString();
            fnt.AppTextColor = DefaultFontColor();
            fnt.AppFillColor = DEFAULT_FILL_COLOR;
            SetAppAttributes(fnt);
        }
        public string GetDefaultFont()
        {
            return DefaultFontString();
        }
    }
    public class SaveOnlyFieldsAttribute : AppAttributes
    {
        private const string DEFAULT_FILL_COLOR = "-16744256";
        public SaveOnlyFieldsAttribute()
            : base()
        {
            AppFontColor fnt = new AppFontColor();
            fnt.FontStr = DefaultFontString();
            fnt.AppTextColor = DefaultFontColor();
            fnt.AppFillColor = DEFAULT_FILL_COLOR;

            SetAppAttributes(fnt);
        }
        public string GetDefaultFont()
        {
            return DefaultFontString();
        }

    }
    public class Postion
    {
        public string StartRow
        {
            get;
            set;
        }
        public string StartCol
        {
            get;
            set;
        }
    }
    public class AppFontColor
    {


        public AppFontColor()
        {

        }

        public AppFontColor(string font, string FillCol, string TxtCol)
        {
            FontStr = font;
            AppTextColor = TxtCol;
            AppFillColor = FillCol;
        }

        private string mFontStr;
        public string FontStr
        {

            get { return mFontStr; }
            set
            {   // if the default is set and the setting value = null, keep the default. 
                if (value != null) mFontStr = value;
            }
        }


        //public string AppFontName
        //{
        //    get;
        //    set;

        //}
        //public int AppFontSize
        //{
        //    get;
        //    set;
        //}
        public string AppTextColor
        {
            get;
            set;
        }

        public string AppFillColor
        {
            get;
            set;
        }
    }


}
