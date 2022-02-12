

using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

///@author Tanja Koivunen
///@version 3.3.2020
///
/// <summary>
/// Peli, jossa hahmo tavoittelee minuutissa osumia taivaalta putoileviin lumihiutaleisiin niin paljon kuin ennättä. 
/// Jokaisesta osumasta saa pisteen. Yhdestä osumsta lumihiutale vaihtaa väriään ja kahdesta osumasta lumihiutale katoaa.
/// 
/// </summary>
/// 
/// 
public class Fysiikkapeli1 : PhysicsGame
{
    Image taustakuva = LoadImage("taustakuva4");
   
    PhysicsObject lumihiutale;
    
    private IntMeter pisteLaskuri;


    public override void Begin()
    {
        
               
        LuoPeliYmparisto();
        AloitaPeli();
        Timer.CreateAndStart(60, AikaLoppui);

        Level.Background.Image = taustakuva;

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }



    private void LuoPeliYmparisto()
    {
        lumihiutale = new PhysicsObject(40.0, 40.0);
        lumihiutale.Shape = Shape.Star;
        lumihiutale.X = 0.0;
        lumihiutale.Y = 200.0;
        Gravity = new Vector(0.0, -20.0); //Painovoiman lisäys, kappaleet alaspäin joten x:n avona 0
        Add(lumihiutale);

        PhysicsObject koira = new PhysicsObject(30.0, 50.0);
        koira.Shape = Shape.Heart;
        koira.Color = Color.Beige;
        koira.X = -100.0;
        koira.Y = -200.0;
        koira.Restitution = 1.0;
        koira.Image = LoadImage("Lotta");
        Add(koira);

        Level.Size = new Vector(800, 600); 
        SetWindowSize(800, 600); // omat mitat,  oletuksena muutoin näytön kokoinen ikkuna
        Level.CreateBorders();
        Camera.ZoomToLevel();

        for (int i = 0; i < 5; i++) // luodaan viisi lumihiutaletta
        {
            Lumihiutale lumihiutale = new Lumihiutale(40, 40, new Color[] {Color.White, Color.Lavender, Color.Magenta});
            lumihiutale.Shape = Shape.Star;
            lumihiutale.Position = RandomGen.NextVector(Level.BoundingRect);
            Add(lumihiutale);

        }
        AddCollisionHandler<PhysicsObject, Lumihiutale>(koira, TormattiinLumihiutaleeseen); // lisää törmäyksen käsittelijä


        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, null, koira, new Vector(-800, 0));
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, null, koira, new Vector(800, 0));
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, null, koira, new Vector(0, -800));
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, null, koira, new Vector(0, 800));

    }
     private void TormattiinLumihiutaleeseen(PhysicsObject koira, Lumihiutale kohde)
     {
        kohde.OtaVastaanOsuma();
        pisteLaskuri.Value += 1;

    }

        


    private void AloitaPeli()
    {
        Vector impulssi = new Vector(-20.0, -90.0);
        lumihiutale.Hit(impulssi);
        LuoPisteLaskuri();

    }

    private void Liikuta(PhysicsObject koira, Vector Suunta) // liikuteltava olio ja suunta)
    {
        koira.Push(Suunta);
    }


    private void LuoPisteLaskuri()
    {
        pisteLaskuri = new IntMeter(0);
        pisteLaskuri.MaxValue = 12;
                
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



    private void LuoAikaLaskuri()
    {
        Timer aikaLaskuri = new Timer();
        aikaLaskuri.Interval = 45;
        aikaLaskuri.Timeout += AikaLoppui;
        aikaLaskuri.Start(1);

        Label aikaNaytto = new Label();
        aikaNaytto.TextColor = Color.White;
        aikaNaytto.DecimalPlaces = 1;
        aikaNaytto.BindTo(aikaLaskuri.SecondCounter);
        Add(aikaNaytto);

        return;
    }

    private void AikaLoppui()
    {
        MessageDisplay.Add("Aika loppui");

        
    }

}



