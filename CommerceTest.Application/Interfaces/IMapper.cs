namespace CommerceTest.Application.Interfaces;

public interface IMapper<in TSrc, out TDest> where TSrc: class where TDest: class
{
    TDest Map(TSrc source);   
}