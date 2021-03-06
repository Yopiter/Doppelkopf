﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Doppelkopf_Client
{
    class Spieler
    {
        public string Name;
        public int Punkte;
        public List<Zustand> ZustandsList;
        public int Kartenzahl;
        public Button btKarte;
        public TextBox tbLabel;
        public int AbsolutePositionAufServer;
        List<PictureBox> Zustandsboxen = new List<PictureBox>();
        PbAnornung PbOrdnung;

        Size PbSize;

        public Spieler(string name, Button button, TextBox labelbox, int Position, PbAnornung Anordnung)
        {
            Name = name;
            Punkte = 0;
            ZustandsList = new List<Zustand>();
            btKarte = button;
            tbLabel = labelbox;
            PbOrdnung = Anordnung;
            AbsolutePositionAufServer = Position;
            Kartenzahl = 12;
        }

        public PictureBox AddZustand(Zustand NeuerZustand)
        {
            PictureBox newBox = new PictureBox();
            switch (NeuerZustand)
            {
                case (Zustand.Re):
                    newBox.Image = Main.GetImage("Ress\\Icons\\re.png");
                    break;
                case (Zustand.Kontra):
                    newBox.Image = Main.GetImage("Ress\\Icons\\contra.png");
                    break;
                case (Zustand.HochzeitAlte):
                    newBox.Image = Main.GetImage("Ress\\Icons\\Braut.png");
                    break;
                case (Zustand.HochzeitOhneAlte):
                    newBox.Image = Main.GetImage("Ress\\Icons\\Brautpaar.png");
                    break;
                default:
                    MessageBox.Show("Oh no, Player " + Name + " seems to be retarted. He thinks he can be " + NeuerZustand.ToString());
                    break;
            }
            newBox.Location = GetPositionFuerNeuePicBox();
            newBox.SizeMode = PictureBoxSizeMode.Zoom;
            Zustandsboxen.Add(newBox);
            ZustandsList.Add(NeuerZustand);
            return newBox;
        }

        private Point GetPositionFuerNeuePicBox() //Baut eventuell ein Hakenkreuz, wenn alle Spieler mehrere Statusanzeigen haben :S #casualNazism
        {
            Point StartPoint;
            int AbstandName = 10;
            int AbstandAnderePB = 5;
            switch (PbOrdnung)
            {
                case (PbAnornung.horizontalLinks): //Spieler unten (Nutzer selbst)
                    StartPoint = tbLabel.Location;
                    StartPoint.X += tbLabel.Width;
                    StartPoint.Y -= AbstandName+PbSize.Height; //Startpunkt für erste PB erreicht
                    StartPoint.X -= Zustandsboxen.Count * (PbSize.Width+AbstandAnderePB);
                    return StartPoint;
                case (PbAnornung.horizontalRechts): //Spieler oben (Nutzer gegenüber)
                    StartPoint = tbLabel.Location;
                    StartPoint.Y += AbstandName + tbLabel.Height+AbstandName;
                    StartPoint.X -= Zustandsboxen.Count * (PbSize.Width + AbstandAnderePB);
                    return StartPoint;
                case (PbAnornung.vertikalAuf): //Spieler links
                    StartPoint = tbLabel.Location;
                    StartPoint.Y -= AbstandName + PbSize.Height;
                    StartPoint.Y -= Zustandsboxen.Count * (PbSize.Height + AbstandAnderePB);
                    return StartPoint;
                case (PbAnornung.vertikalAb): //Spieler rechts
                    StartPoint = tbLabel.Location;
                    StartPoint.Y += AbstandName + tbLabel.Height;
                    StartPoint.X += tbLabel.Width - PbSize.Width;
                    StartPoint.Y += Zustandsboxen.Count * (PbSize.Height + AbstandAnderePB);
                    return StartPoint;
                default:
                    MessageBox.Show("Spieler " + Name + " hat eine ungültige Anordnung für seine Statusboxen erhalten: " + PbOrdnung.ToString(), "Fehler in Spieler.cs->GetPositionFuerNeuePicBox()");
                    return new Point(0, 0);
            }
        }

        public void SetPbSize(Size newSize)
        {
            PbSize = newSize;
        }

        public void KarteLegenLassen(Karte K)
        {
            btKarte.Image = Main.GetImage(K.GetImagePath());
            btKarte.Visible = true;
            Kartenzahl--;
        }
    }
}
