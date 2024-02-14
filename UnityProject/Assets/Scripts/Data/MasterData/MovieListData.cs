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
            Release     = 10,
        }
    }
}