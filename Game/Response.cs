namespace Game
{
    public class Response<T>
    {
        public T Result { get; set; }
        public bool Success { get; set; }
    }
}