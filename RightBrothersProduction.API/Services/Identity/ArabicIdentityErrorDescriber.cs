using Microsoft.AspNetCore.Identity;

public class ArabicIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError PasswordTooShort(int length)
    {
        return new IdentityError
        {
            Code = nameof(PasswordTooShort),
            Description = $"كلمة المرور يجب أن تحتوي على الأقل {length} أحرف."
        };
    }

    public override IdentityError PasswordRequiresUpper()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresUpper),
            Description = "كلمة المرور يجب أن تحتوي على حرف كبير واحد على الأقل (A-Z)."
        };
    }

    public override IdentityError PasswordRequiresLower()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresLower),
            Description = "كلمة المرور يجب أن تحتوي على حرف صغير واحد على الأقل (a-z)."
        };
    }

    public override IdentityError PasswordRequiresDigit()
    {
        return new IdentityError
        {
            Code = nameof(PasswordRequiresDigit),
            Description = "كلمة المرور يجب أن تحتوي على رقم واحد على الأقل (0-9)."
        };
    }
}