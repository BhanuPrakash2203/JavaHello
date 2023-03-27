
namespace IAFG.IA.VE.Impression.Core.Types.Enums
{
    public enum Language
    {
        Unknown = 0,
        English = 1,
        French = 2
    }

    public enum IsoPdfVersion
    {
        Unknown = 0,
        Pdf11 = 1,      // PDF version 1.1. Suitable for legacy viewers. 
        Pdf12 = 2,      // PDF version 1.2. This is the native file format of Acrobat 3.0. 
        Pdf13 = 3,      // PDF version 1.3. This is the native file format of Acrobat 4.0.     
        Pdf14 = 4,      // PDF version 1.4. 
        Pdf15 = 5,      // PDF version 1.5. 
        Pdf16 = 6,      // PDF version 1.6. 
        Pdf17 = 7,      // PDF version 1.7. 
        Pdfa1A = 8,     // PDF/A-1a level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa1B = 9,     // PDF/A-1b level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa2A = 10,    // PDF/A-2a level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa2B = 11,    // PDF/A-2b level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa2U = 12,    // PDF/A-2u level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa3A = 13,    // PDF/A-3a level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa3B = 14,    // PDF/A-3b level, see http://en.wikipedia.org/wiki/PDF/A 
        Pdfa3U = 15     // PDF/A-3u level, see http://en.wikipedia.org/wiki/PDF/A     
    }

    public enum ApplicationMode
    {
        Unknown = 0,
        HeadOffice = 1,
        AgencyStaff = 2,
        Agent = 3
    }
}
