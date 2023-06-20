namespace Supreme.Domain.Constants;

public static class ValidationMessageKeys
{
    public const string _requestBodyRequired = "request.body.should.not.be.null";
    public const string _userIdShouldBeGreaterThan = "userid.should.be.greater.than";

    public static class FooMessages
    {
        public const string _nameRequired = "name.should.not.be.null";
        public const string _nameLengthShouldBeGreaterThanOrEqualTo = "name.length.should.be.greater.than.or.equal.to";
        public const string _nameLengthShouldBeLessThanOrEqualTo = "name.length.should.be.smaller.than.or.equal.to";
        public const string _idShouldBeGreaterThan = "id.should.be.greater.than";
    }
}