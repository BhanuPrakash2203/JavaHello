namespace IAFG.IA.VE.Impression.Illustration.Test.Fake
{
    /// <summary>
    /// Summary description for FakeMasterReport.
    /// </summary>
    partial class FakeMasterReport
    {
        private GrapeCity.ActiveReports.SectionReportModel.Detail detail;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        #region ActiveReport Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FakeMasterReport));
            this.detail = new GrapeCity.ActiveReports.SectionReportModel.Detail();
            this.pageHeader1 = new GrapeCity.ActiveReports.SectionReportModel.PageHeader();
            this.LogoPageHeader = new GrapeCity.ActiveReports.SectionReportModel.Picture();
            this.pageFooter1 = new GrapeCity.ActiveReports.SectionReportModel.PageFooter();
            ((System.ComponentModel.ISupportInitialize)(this.LogoPageHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // detail
            // 
            this.detail.Height = 0.5208333F;
            this.detail.Name = "detail";
            // 
            // pageHeader1
            // 
            this.pageHeader1.BackColor = System.Drawing.Color.Gainsboro;
            this.pageHeader1.Controls.AddRange(new GrapeCity.ActiveReports.SectionReportModel.ARControl[] {
            this.LogoPageHeader});
            this.pageHeader1.Height = 0.661F;
            this.pageHeader1.Name = "pageHeader1";
            // 
            // LogoPageHeader
            // 
            this.LogoPageHeader.Height = 0.505F;
            this.LogoPageHeader.HyperLink = null;
            this.LogoPageHeader.ImageData = ((System.IO.Stream)(resources.GetObject("LogoPageHeader.ImageData")));
            this.LogoPageHeader.Left = 0.108F;
            this.LogoPageHeader.Name = "LogoPageHeader";
            this.LogoPageHeader.PictureAlignment = GrapeCity.ActiveReports.SectionReportModel.PictureAlignment.TopLeft;
            this.LogoPageHeader.SizeMode = GrapeCity.ActiveReports.SectionReportModel.SizeModes.Zoom;
            this.LogoPageHeader.Tag = "Logo";
            this.LogoPageHeader.Top = 0F;
            this.LogoPageHeader.Width = 1.023F;
            // 
            // pageFooter1
            // 
            this.pageFooter1.BackColor = System.Drawing.Color.Gainsboro;
            this.pageFooter1.CanShrink = true;
            this.pageFooter1.Height = 0.868F;
            this.pageFooter1.Name = "pageFooter1";
            // 
            // FakeMasterReport
            // 
            this.MasterReport = true;
            this.PageSettings.PaperHeight = 11F;
            this.PageSettings.PaperWidth = 8.5F;
            this.PrintWidth = 7.5F;
            this.Sections.Add(this.pageHeader1);
            this.Sections.Add(this.detail);
            this.Sections.Add(this.pageFooter1);
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Arial; font-style: normal; text-decoration: none; font-weight: norma" +
            "l; font-size: 10pt; color: Black", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 16pt; font-weight: bold", "Heading1", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-family: Times New Roman; font-size: 14pt; font-weight: bold; font-style: ita" +
            "lic", "Heading2", "Normal"));
            this.StyleSheet.Add(new DDCssLib.StyleSheetRule("font-size: 13pt; font-weight: bold", "Heading3", "Normal"));
            ((System.ComponentModel.ISupportInitialize)(this.LogoPageHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }
        #endregion

        private GrapeCity.ActiveReports.SectionReportModel.PageHeader pageHeader1;
        private GrapeCity.ActiveReports.SectionReportModel.Picture LogoPageHeader;
        private GrapeCity.ActiveReports.SectionReportModel.PageFooter pageFooter1;
    }
}
