using Domain.Common;

namespace Domain.Entities;

public class Transfer : Entity
{
    public Guid PlayerId { get; private set; }
    public Guid FromTeamId { get; private set; }
    public Guid ToTeamId { get; private set; }
    public DateTime TransferDate { get; private set; }
    public decimal Fee { get; private set; }

    private Transfer() { } // EF Core

    public Transfer(Guid playerId, Guid fromTeamId, Guid toTeamId, DateTime transferDate, decimal fee)
    {
        if (playerId == Guid.Empty)
            throw new ArgumentException("PlayerId cannot be empty.", nameof(playerId));
        if (fromTeamId == Guid.Empty)
            throw new ArgumentException("FromTeamId cannot be empty.", nameof(fromTeamId));
        if (toTeamId == Guid.Empty)
            throw new ArgumentException("ToTeamId cannot be empty.", nameof(toTeamId));
        if (fromTeamId == toTeamId)
            throw new ArgumentException("FromTeamId and ToTeamId cannot be the same.", nameof(toTeamId));
        if (fee < 0)
            throw new ArgumentException("Fee cannot be negative.", nameof(fee));

        PlayerId = playerId;
        FromTeamId = fromTeamId;
        ToTeamId = toTeamId;
        TransferDate = transferDate;
        Fee = fee;
    }
}
