namespace H00N
{
    [System.Serializable]
    public class OptOption<T>
    {
        public OptOption() { }
        public OptOption(T positiveOption, T negativeOption)
        {
            PositiveOption = positiveOption;
            NegativeOption = negativeOption;
        }

        public T this[bool option] => GetOption(option);

        public T PositiveOption;
        public T NegativeOption;

        public T GetOption(bool decision) => decision ? PositiveOption : NegativeOption;
    }
}