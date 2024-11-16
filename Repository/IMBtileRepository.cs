namespace MobileProject.Repository
{
    public interface IMbtileRepository
    {
        byte[]? ReadTile(int tileColumn, int tileRow, int zoomLevel);
        //byte[]? ReadFirstTile();
        (int Min,int Max)? GetZoomLevelRange();
        //void AddTile(int tileColumn,int tileRow,int zoomLevel, byte[]tileData);
    }
}
