namespace Doppelkopf_Client
{
    public enum Farben
    {
        Schellen = 0,
        Herz = 1,
        Blatt = 2,
        Eichel = 3
    }

    public enum Kartenwert
    {
        Neun = 0,
        König = 1,
        Unter = 2,
        Ober = 3,
        Zehn = 4,
        Ass = 5
    }

    public enum Spielmodus
    {
        Normal,
        Hochzeit,
        StillesSolo
    }

    enum Zustand
    {
        Re,
        HochzeitAlte,
        HochzeitOhneAlte,
        Solo,
        Kontra
    }

    enum PbAnornung
    {
        vertikalAuf,
        horizontalRechts,
        vertikalAb,
        horizontalLinks
    }
}
