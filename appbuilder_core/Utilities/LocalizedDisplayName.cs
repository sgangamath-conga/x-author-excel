namespace Apttus.XAuthor.Core
{
    /// <summary>
    /// Overrides DisplayNameAttribute to display labels from resource file for property grid
    /// </summary>
    public class LocalDisplayNameAttribute : System.ComponentModel.DisplayNameAttribute
    {
        readonly string resourceName;
        public LocalDisplayNameAttribute(string resourceName)
            : base()
        {
            this.resourceName = resourceName;
        }

        public override string DisplayName
        {
            get
            {
                return ApttusResourceManager.GetInstance.GetResource(this.resourceName);
            }
        }
    }
}
