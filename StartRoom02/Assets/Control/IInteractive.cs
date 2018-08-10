
public interface IInteractive
{
    // для передачи параметров от контрола при инициализации
    void setState( State s);
    // для получения параметров контролом при сохранении и работе
    State getState();
}
