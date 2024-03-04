namespace data.master
{
    /// <summary>
    /// ムービーデータまとめ型
    /// </summary>
    public class MovieListData : MasterListDataBase<MovieData, MovieData.Data>
    {
        public enum MovieType
        {
            Title       = 1,
            Feedback    = 10,
            Cansel      = 11,
            Release     = 12,
            Recreate    = 14,
        }
    }
}