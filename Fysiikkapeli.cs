
using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

///@author Tanja Koivunen
///@version 20.4.2020
///
/// <summary>
/// Peli, jossa hahmo tavoittelee osumia taivaalta putoileviin lumihiutaleisiin. Aikaa on yksi minuutti. 
/// Jokaisesta osumasta saa pisteen. Lumihiutaleita on kahdenlaisia: valkoisia jäälumihiutaleita 
/// ja tähdenmuotoisia väriävaihtavia lumihiutaleita. Lumihiutaleet katoavat kolmannesta osumasta. 
/// Hahmoa kohti tippuu myös lumipalloja ja jääpuikkoja, joita pelissä tulee välttää, 
/// sillä lumipallon osuminen vähentää pisteen ja jääpuikon osuminen kaksi pistettä.
/// </summary>


public class Fysiikkapeli2 : PhysicsGame
{
    private Image taustakuva = LoadImage("taustakuva1");
    private DoubleMeter alaspainLaskuri;
    private Timer aikaLaskuri;
    private IntMeter pisteLaskuri;
    private int[] pisteet;
    private PhysicsObject lumihiutale;
    private List<Label> valikonKohdat;

    const int TAHTIEN_MAARA = 3;
    

    public override void Begin()
    {

        pisteet = new int[] { 0, 0, 0 };
        AlkuValikko();

        Mouse.IsCursorVisible = true;

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    /// <summary>
    /// Luodaan pelin alkuvalikko.
    /// </summary>
    private void AlkuValikko()
    {
        Level.Background.Image = LoadImage("taustakuva2");
        MultiSelectWindow alkuValikko = new MultiSelectWindow("Pelin alkuvalikko",
        "Aloita peli", "Lopeta");
        alkuValikko.AddItemHandler(0, AloitaPeli);
        alkuValikko.AddItemHandler(1, Exit);
        alkuValikko.Color = Color.RosePink;

        Add(alkuValikko);

        MessageDisplay.Add("Tervetuloa pelamaan! " + "\n" +
        "Kerää mahdollisimman monta osumaa " + "\n" +
        "jäälumihiutaleisiin ja tähtilumihiutaleisiin." + "\n" +
        "Aikaa on yksi minuutti. Liikuta koiraa nuolinäppäimillä. " + "\n" +
        "Varo lumipalloja (-1 p.) ja jääpuikkoja (-2 p.)," + "\n" +
        "sillä niihin osuminen vähentää pisteitä.");
        MessageDisplay.X = Screen.Left + 600;
        MessageDisplay.Y = Screen.Top - 250;
        MessageDisplay.BackgroundColor = Color.Transparent;
        MessageDisplay.TextColor = Color.MediumVioletRed;
        MessageDisplay.Font = Font.Default;


    }

    /// <summary>
    /// Luodaan pelin aloitus ja ajastimet.
    /// </summary>
    private void AloitaPeli()
    {

        LuoPeliYmparisto();
        LuoLumihiutale();
        LuoKoira();
        LuoLumipallo();
        LuoAikaLaskuri();
        LuoPisteLaskuri();
        LaskeAlaspain();

        Timer.CreateAndStart(10, LisaaLumihiutaleita);
        Timer.CreateAndStart(8, LuoLumihiutale);
        Timer.CreateAndStart(13, LuoLumipallo);
        Timer.CreateAndStart(12, LuoLumipalloAani);
        Timer.CreateAndStart(16, LuoJaapuikko);

        IsPaused = false;

    }
    /// <summary>
    /// Luodaan pelikenttä ja sen mitat.
    /// </summary>
    private void LuoPeliYmparisto()
    {
        Level.Background.Image = taustakuva;
        Level.Size = new Vector(800, 600);
        SetWindowSize(800, 600);
        Level.CreateBorders();
        Camera.ZoomToLevel();


    }
    /// <summary>
    /// Luodaan pelattavaksi hahmoksi koira ja sen ominaisuudet.
    /// </summary>
    private void LuoKoira()
    {
        PhysicsObject koira = PhysicsObject.CreateStaticObject(40.0, 50.0);
        koira.Image = LoadImage("koira1.png");
        koira.Mass = 1.0;
        koira.X = 0.0;
        koira.Y = -180.0;
        koira.Restitution = 0.5;
        koira.AngularDamping = 0.5;
        koira.MaxVelocity = 250;
        Add(koira);

        AddCollisionHandler<PhysicsObject, Lumihiutale>(koira, TormattiinLumihiutaleeseen); // lisää törmäyksen käsittelijä

        AddCollisionHandler(koira, "miinus", LumipallonOsuma);
        AddCollisionHandler(koira, "jaapuikkomiinus", OsumaJaapuikkoon);

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, null, koira, new Vector(-800, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, null, koira, new Vector(800, 0));
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, null, koira, new Vector(0, -800));
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, null, koira, new Vector(0, 800));
    }


    /// <summary>
    /// Luodaan lumihiutale ja sen ominaisuudet.
    /// </summary>
    private void LuoLumihiutale()
    {
        lumihiutale = new Lumihiutale(40.0, 40.0, new Color[] { Color.White, Color.Lavender, Color.Magenta });
        lumihiutale.Image = LoadImage("lumihiutale");
        lumihiutale.X = RandomGen.NextInt((int)Level.Left, (int)Level.Right);
        lumihiutale.Y = Level.Top;
        Gravity = new Vector(0.0, -60.0);
        Add(lumihiutale);
    }


    /// <summary>
    /// Luodaan kolme tähtilumihiutaletta, jotka vaihtavat väriä osumista.
    /// </summary>
    private void LisaaLumihiutaleita()
    {
        for (int i = 0; i < TAHTIEN_MAARA; i++)
        {
            lumihiutale = new Lumihiutale(30, 30, new Color[] { Color.Pink, Color.Lavender, Color.Magenta });
            lumihiutale.Shape = Shape.Star;
            lumihiutale.X = RandomGen.NextInt((int)Level.Left, (int)Level.Right);
            lumihiutale.Y = Level.Top;
            Add(lumihiutale);
        }

    }


    /// <summary>
    /// Luodaan lumipallo.
    /// </summary>
    private void LuoLumipallo()
    {
        PhysicsObject lumipallo = new PhysicsObject(40, 40);
        lumipallo.X = RandomGen.NextInt((int)Level.Left, (int)Level.Right);
        lumipallo.Y = Level.Top;
        Gravity = new Vector(0.0, -100.0);
        lumipallo.Shape = Shape.Circle;
        lumipallo.Tag = "miinus";
        Add(lumipallo);
    }

    /// <summary>
    /// Luodaan lumipallon tekemisen ääni.
    /// </summary>
    private void LuoLumipalloAani()
    {
        SoundEffect lumipalloAani = LoadSoundEffect("Lumipalloaani");
        lumipalloAani.Play();
    }


    /// <summary>
    /// Luodaan jaapuikko.
    /// </summary>
    private void LuoJaapuikko()
    {
        PhysicsObject jaapuikko = new PhysicsObject(25, 50);
        jaapuikko.X = RandomGen.NextInt((int)Level.Left, (int)Level.Right);
        jaapuikko.Y = Level.Top;
        jaapuikko.Shape = Shape.Diamond;
        jaapuikko.Image = LoadImage("jaapuikko");
        jaapuikko.Tag = "jaapuikkomiinus";
        Add(jaapuikko);
    }


    /// <summary>
    /// Luodaan törmäyksenkäsittelijä tilanteeseen, jossa pelaajahahmo koira törmää lumihiutaleeseen.
    /// </summary>
    /// <param name="koira">Törmääjä</param>
    /// <param name="kohde">Törmäyksen kohde Lumihiutale</param>
    private void TormattiinLumihiutaleeseen(PhysicsObject koira, Lumihiutale kohde)
    {
        kohde.OtaVastaanOsuma();
        pisteLaskuri.Value += 1;
        pisteet[0] += 1;

    }


    /// <summary>
    /// Luodaan törmäyksenkäsittelijä tilanteeseen, jossa koiraan osuu lumipallo.
    /// </summary>
    /// <param name="koira">Törmääjä</param>
    /// <param name="lumipallo">Törmäyksen kohde</param>
    private void LumipallonOsuma(PhysicsObject koira, PhysicsObject lumipallo)
    {
        CollisionHandler.DestroyTarget(koira, lumipallo);
        pisteLaskuri.Value -= 1;
        pisteet[1] += 1;
    }


    /// <summary>
    /// Luodaan törmäyksenkäsittelijä tilanteeseen, jossa koiraan osuu jääpuikko.
    /// </summary>
    /// /// <param name="koira">Törmääjä</param>
    /// <param name="jaapuikko">Törmäyksen kohde</param>
    private void OsumaJaapuikkoon(PhysicsObject koira, PhysicsObject jaapuikko)
    {
        CollisionHandler.DestroyTarget(koira, jaapuikko);
        pisteLaskuri.Value -= 2;
        pisteet[2] += 1;
    }


    /// <summary>
    /// Luodaan hahmon liikuttaminen. Liikuteltava hahmo koira ja suunta.
    /// </summary>
    /// <param name="koira">Liikuteltava olio</param>
    /// <param name="Suunta">Liikkumissuunta</param>
    private void Liikuta(PhysicsObject koira, Vector Suunta)
    {
        koira.Push(Suunta);
    }


    /// <summary>
    /// Luodaan pisteitä laskeva laskuri.
    /// </summary>
    private void LuoPisteLaskuri()
    {

        pisteLaskuri = new IntMeter(0);
        pisteLaskuri.MaxValue = 100;
        pisteLaskuri.MinValue = -50;
        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Lavender;
        pisteNaytto.BorderColor = Level.Background.Color;
        pisteNaytto.Color = Color.White;
        pisteNaytto.Title = "Pisteet";
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
        return;
    }


    /// <summary>
    /// Luodaan käytettävissä olevalle ajalle laskuri.
    /// </summary>
    private void LuoAikaLaskuri()
    {
        alaspainLaskuri = new DoubleMeter(60);

        aikaLaskuri = new Timer();
        aikaLaskuri.Interval = 0.1;
        aikaLaskuri.Timeout += LaskeAlaspain;
        aikaLaskuri.Start();

        Label aikaNaytto = new Label();
        aikaNaytto.TextColor = Color.MediumVioletRed;
        aikaNaytto.X = Screen.Right - 100;
        aikaNaytto.Y = Screen.Top - 100;
        aikaNaytto.DecimalPlaces = 1;
        aikaNaytto.BindTo(alaspainLaskuri);
        Add(aikaNaytto);
        return;
    }


    /// <summary>
    /// Luodaan pelin loppuminen kun aika loppuu.
    /// </summary>
    private void LaskeAlaspain()
    {
        alaspainLaskuri.Value -= 0.1;

        if (alaspainLaskuri.Value <= 0)
        {

            aikaLaskuri.Stop();
            pisteLaskuri.Stop();
            IsPaused = true;

            PelinLoppu();
        }
    }


    /// <summary>
    /// Luodaan pelin loppuilmoitus ja näytetään yhteenveto pisteistä.
    /// Luodaan loppuvalikko.
    /// </summary>
    private void PelinLoppu()
    {

        Level.Background.Image = LoadImage("taustakuva2");
        MessageDisplay.TextColor = Color.Violet;
        MessageDisplay.Color = Color.Transparent;
        MessageDisplay.Font = Font.Default;
        MessageDisplay.Add("Aika loppui. Pistemääräsi oli " + pisteLaskuri + " pistettä." + "\n" +
        "Sait " + pisteet[0] + " osumaa lumihiutaleisiin." + "\n" +
        "Lumipallo osui " + pisteet[1] + " kertaa." + "\n" +
        "Jääpuikko osui " + pisteet[2] + " kertaa." + "\n" +
        "Osumia tippuviin kohteisiin: " + MontakoSekunnissa(pisteet) + " kertaa sekunnissa.");

        valikonKohdat = new List<Label>();

        Label kohta1 = new Label("Aloita uusi peli");
        kohta1.Position = new Vector(0, 0);
        valikonKohdat.Add(kohta1);

        Label kohta2 = new Label("Lopeta peli");
        kohta2.Position = new Vector(0, -40);
        valikonKohdat.Add(kohta2);


        foreach (Label valikonKohta in valikonKohdat)
        {
            Add(valikonKohta);
        }
        Mouse.ListenOn(kohta1, MouseButton.Left, ButtonState.Pressed, AloitaAlusta, null);
        Mouse.ListenOn(kohta2, MouseButton.Left, ButtonState.Pressed, Exit, null);
    }


    /// <summary>
    /// Aliohjelma, joka palauttaa funktiolla montako osumaa tuli sekunnissa
    /// taivaalta tippuviin pelin kohteisiin.
    /// </summary>
    /// <param name="pisteet">Osumien pistetaulukko</param>
    /// <returns>Osumien määrä sekunnissa</returns>
    private double MontakoSekunnissa(int[] pisteet)
    {
        double summa = 0;
        for (int i = 0; i < pisteet.Length; i++)
        {
            summa += pisteet[i];
        }
        return Math.Round(summa /60, 2);
    }

    
    /// <summary>
    /// Aloittaa pelin alusta.
    /// </summary>
    private void AloitaAlusta()
    {
        ClearAll();

        Begin();
    }
}









