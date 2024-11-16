using Microsoft.Data.Sqlite;
namespace MobileProject.Repository
{

    public class MBTileRepository:IMbtileRepository
    {

        private readonly string connectionString= new SqliteConnectionStringBuilder
        {
            DataSource = "./osm.mbtiles",
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared,
        }.ToString();

        private const string TableImages = "images";

        private const string ColumnTileId = "tile_id";

        private const string TableMap = "map";

        private const string ColumnTileColumn = "tile_column";

        private const string ColumnTileRow = "tile_row";

        private const string ColumnZoomLevel = "zoom_level";

        private const string ColumnTileData = "tile_data";

        private const string TableMetadata = "metadata";

        private const string ColumnMetadataName = "name";

        private const string ColumnMetadataValue = "value";

        private const string ReadTileIdCommandText = $"SELECT {ColumnTileId} " +
            $"FROM {TableMap} WHERE (({ColumnZoomLevel} = @{ColumnZoomLevel}) AND ({ColumnTileColumn} = @{ColumnTileColumn}) AND ({ColumnTileRow} = @{ColumnTileRow}))";
        
        private const string ReadTileDataCommandText = $"SELECT {ColumnTileData} FROM {TableImages} WHERE {ColumnTileId} = @{ColumnTileId}";


  
        // zipped
        public byte[]? ReadTile(int tileColumn, int tileRow, int zoomLevel)
        {
            using var connection = new SqliteConnection(connectionString);
            byte[]? result = null;
            string? tileId = null;

            using (var command = new SqliteCommand(ReadTileIdCommandText, connection))
            {
                command.Parameters.AddRange(
                [
                    new SqliteParameter($"@{ColumnTileColumn}", tileColumn),
                    new SqliteParameter($"@{ColumnTileRow}", tileRow),
                    new SqliteParameter($"@{ColumnZoomLevel}", zoomLevel),
                ]);
                connection.Open();
                using (var dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        tileId = (string)dr[0];
                    }

                    dr.Close();
                }

                if (tileId == null)
                {
                    return null;
                }
            }

            {
                using var command = new SqliteCommand(ReadTileDataCommandText, connection);
                command.Parameters.Add(new SqliteParameter($"@{ColumnTileId}", tileId));
                connection.Open();
                using var dr = command.ExecuteReader();
                if (dr.Read())
                {
                    result = (byte[])dr[0];
                }

                dr.Close();
            }

            return result;
        }
       


        public (int Min, int Max)? GetZoomLevelRange()
        {
            using var connection = new SqliteConnection(connectionString);
            using var command = new SqliteCommand($"SELECT MIN({ColumnZoomLevel}), MAX({ColumnZoomLevel}) FROM {TableMap}", connection);

            connection.Open();
            using var dr = command.ExecuteReader();

            (int Min, int Max)? result = null;
            if (dr.Read())
            {
                result = (Min: dr.GetInt32(0), Max: dr.GetInt32(1));
            }

            dr.Close();

            return result;
        }





        private void ExecuteSqlQuery(string commandText)
        {
            using var connection = new SqliteConnection(connectionString);
            using var command = new SqliteCommand(commandText, connection);
            connection.Open();
            command.ExecuteNonQuery();
        }

        
    }
}