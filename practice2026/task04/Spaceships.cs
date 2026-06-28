using System;

public interface ISpaceship
{
    void MoveForward();     
    void Rotate(int angle); 
    void Fire();           
    int Speed { get; }      
    int FirePower { get; }  
}

public class Cruiser : ISpaceship
{
    public int Speed => 50;
    public int FirePower => 100;

    public void MoveForward() => Console.WriteLine("Крейсер медленно продвигается вперед.");
    public void Rotate(int angle) => Console.WriteLine($"Крейсер тяжело разворачивается на {angle} градусов.");
    public void Fire() => Console.WriteLine("Крейсер производит мощнейший залп фотонными торпедами!");
}

public class Fighter : ISpaceship
{
    public int Speed => 100;
    public int FirePower => 30;

    public void MoveForward() => Console.WriteLine("Истребитель на форсаже летит вперед.");
    public void Rotate(int angle) => Console.WriteLine($"Истребитель делает резкий маневр на {angle} градусов.");
    public void Fire() => Console.WriteLine("Истребитель выпускает легкую фотонную ракету.");
}
