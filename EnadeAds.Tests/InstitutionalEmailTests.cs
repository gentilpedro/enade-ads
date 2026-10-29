using EnadeAds.Accounts;

namespace EnadeAds.Tests;

public class InstitutionalEmailTests
{
    [Theory]
    [InlineData("professor@faculdade.edu.br")]
    [InlineData("professor@mit.edu")]
    [InlineData("PROFESSOR@FATEC.SP.EDU.BR")]
    [InlineData("professor@alunos.faculdade.edu.br")]
    public void IsInstitutional_AcceptsEduDomains(string email)
    {
        Assert.True(InstitutionalEmail.IsInstitutional(email));
    }

    [Theory]
    [InlineData("aluno@gmail.com")]
    [InlineData("aluno@educacao.com.br")]
    [InlineData("aluno@edu.exemplo.com")]
    [InlineData("aluno@faculdade.edu.br@gmail.com")]
    [InlineData("edu.br")]
    [InlineData("@edu.br")]
    [InlineData("")]
    [InlineData(null)]
    public void IsInstitutional_RejectsOtherAddresses(string? email)
    {
        Assert.False(InstitutionalEmail.IsInstitutional(email));
    }
}
