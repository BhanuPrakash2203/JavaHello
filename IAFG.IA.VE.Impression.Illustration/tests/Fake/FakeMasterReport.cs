using System;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.ActiveReports.SectionReportModel;
using IAFG.IA.VE.Impression.Core.Types.Reports;
using IAFG.IA.VE.Impression.Core.Types.Styles;

namespace IAFG.IA.VE.Impression.Illustration.Test.Fake
{
    /// <summary>
    /// Summary description for FakeMasterReport.
    /// </summary>
    public partial class FakeMasterReport : GrapeCity.ActiveReports.SectionReport, IReport
    {

        public readonly List<SubReport>  SubReports = new List<SubReport>();

        public FakeMasterReport()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            ReportStart += FormatterReportStart;
        }

        private void FormatterReportStart(object sender, EventArgs e)
        {
            PageSettings.Margins.Top = 0.25F;
            PageSettings.Margins.Bottom = 0.25F;
            PageSettings.Margins.Left = 0.5F;
            PageSettings.Margins.Right = 0.5F;
        }

        public void AddSubReport(IReport subReport)
        {
            var precedent = SubReports.LastOrDefault();
            var sub = new SubReport
                      {
                          Report = subReport as GrapeCity.ActiveReports.SectionReport,
                          Top = precedent?.Top + precedent?.Height ?? 0,
                          Width = PrintWidth
                      };
            SubReports.Add(sub);
            detail.Controls.Add(sub);
            detail.SizeToFit();
        }

        public void DisposeReport()
        {
            foreach (var report in SubReports)
            {
                report.Dispose();
            }
        }

        public IStyleOverride StyleOverride { get; set; }
    }
}
