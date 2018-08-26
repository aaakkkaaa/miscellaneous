
public interface IInteractive
{
    // // Вызывается из Контрола, например при загрузке мира или настройке параметров <action> сценария
    void setState( State s);
    // для получения параметров контролом при сохранении и работе
    State getState();
}
