namespace Boardgames.Data.Models
{
    using System.ComponentModel.DataAnnotations.Schema;

    public class BoardgameSeller
    {
        public int BoardgameId { get; set; }

        [ForeignKey(nameof(BoardgameId))]
        public virtual Boardgame Boardgame { get; set; } = null!;


        public int SellerId { get; set; }

        [ForeignKey(nameof(SellerId))]
        public virtual Seller Seller { get; set; } = null!;
    }
}
