namespace CoinCollectionAtSpeed.Data
{
    public class PlayerControl
    {
        private int Row { get; set; }
        private int Column { get; set; }
        private int RowMax { get; set; }
        private int ColumnMax { get; set; }

        public PlayerControl(int RowMax, int ColumnMax)
        {
            Row = 0;
            Column = 0;
            this.RowMax = RowMax - 1;
            this.ColumnMax = ColumnMax - 1;
        }

        /*Управление.     
        Увеличиваем/уменьшаем значение координат до достижения максимума/минимума.
        При превышении ограничения устанавливаем его противоположность - больше максимума -> минимум, меньше минимума -> максимум.
        Это нужно для циклического движения по игровому полю - дошел до границы, вышел с противоположной.
        */
        public int Down()
            => Row < RowMax ? ++Row : RowToZeroOrMax(0);

        public int Up()
            => Row > 0 ? --Row : RowToZeroOrMax(RowMax);

        public int Right()
            => Column < ColumnMax ? ++Column : ColumnToZeroOrMax(0);

        public int Left()
            => Column > 0 ? --Column : ColumnToZeroOrMax(ColumnMax);

        private int RowToZeroOrMax(int ZM)
        {
            Row = ZM;
            return ZM;
        }
        private int ColumnToZeroOrMax(int ZM)
        {
            Column = ZM;
            return ZM;
        }

        public void Restart()
        {
            Row = 0;
            Column = 0;
        }
    }
}
