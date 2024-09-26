using System.Runtime.Serialization;

namespace Core.Enum
{
    public enum Genre
    {
        [EnumMember(Value = "Action")]
        Action,

        [EnumMember(Value = "Adventure")]
        Adventure,

        [EnumMember(Value = "Animation")]
        Animation,

        [EnumMember(Value = "Comedy")]
        Comedy,

        [EnumMember(Value = "Crime")]
        Crime,

        [EnumMember(Value = "Documentary")]
        Documentary,

        [EnumMember(Value = "Drama")]
        Drama,

        [EnumMember(Value = "Family")]
        Family,

        [EnumMember(Value = "Fantasy")]
        Fantasy,

        [EnumMember(Value = "History")]
        History,

        [EnumMember(Value = "Horror")]
        Horror,

        [EnumMember(Value = "Music")]
        Music,

        [EnumMember(Value = "Mystery")]
        Mystery,

        [EnumMember(Value = "Romance")]
        Romance,

        [EnumMember(Value = "Science Fiction")]
        ScienceFiction,

        [EnumMember(Value = "Thriller")]
        Thriller,

        [EnumMember(Value = "War")]
        War,

        [EnumMember(Value = "Western")]
        Western

    }
}
