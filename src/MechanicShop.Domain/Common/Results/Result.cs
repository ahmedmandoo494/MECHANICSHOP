using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.VisualBasic;

namespace MechanicShop.Domain.Common.Results;

public class Result
{
    public static Success Success => default;
    public static Created Created => default;
    public static Deleted Deleted => default;
    public static Updated Updated => default;

}

public class Result<TValue> : IResult<TValue>
{
    private readonly TValue? _value = default;
    private readonly List<Error>? _errors =null ;

    public bool IsSuccess{get;}
    public bool IsError => !IsSuccess;

    public TValue Value => IsSuccess ?_value! : default!;
    public List<Error>? Errors => IsError? _errors : null;

    public Error TopError => (_errors?.Count > 0)? _errors[0]:default;

    [JsonConstructor]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("for serializer only",true)]
    public Result(TValue? value, List<Error>? errors , bool isSuccess)
    {
        if (isSuccess)
        {
            _value = value ??  throw new ArgumentNullException(nameof(value));
            _errors =[];
            IsSuccess = true;
        }
        else
        {
            _errors = errors ?? throw new ArgumentNullException("Provid eat lest one error.",nameof(errors));
            _value = default;
            IsSuccess = false;
        }
    }
    private Result(Error error)
    {
        _errors =[error];
        IsSuccess = false;
    }
    private Result(List<Error> errors)
    {
        if(errors is null || errors.Count == 0)
        {
            throw new ArgumentNullException("Cannot Create an ErrorOr<TValue> From an Empty collection of errers, Provid eat lest one error.",nameof(errors));
        }
        _errors = errors;
        IsSuccess = false;
    }
    private Result(TValue value)
    {
        if( value is null )
        {
            throw new ArgumentNullException(nameof(value));
        }
        _value = value;
        IsSuccess = true;
    }
    
    public TNextValue Match<TNextValue>(Func<TValue , TNextValue> onValue,Func<List<Error> , TNextValue> onError) 
    => IsSuccess ? onValue(Value) : onError(Errors!);

    public static implicit operator Result<TValue>(List<Error> errors) => new(errors);
    public static implicit operator Result<TValue>(Error error) => new(error);
    public static implicit operator Result<TValue>(TValue value) => new(value);

}

public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Deleted;
public readonly record struct Updated;

