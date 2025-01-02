using StrawberryShake.Serialization;

namespace CommerceTest.Infrastructure.BigCommerce.Serializers;

public class BigDecimalSerializer : ScalarSerializer<decimal>
{
    public BigDecimalSerializer()
        : base(
            "BigDecimal")
    {
    }
}
