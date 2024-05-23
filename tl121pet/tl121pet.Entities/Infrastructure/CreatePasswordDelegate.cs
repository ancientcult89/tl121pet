namespace tl121pet.Entities.Infrastructure
{
    public delegate void CreatePasswordDelegate(string password, out byte[] passwordHash, out byte[] passwordSalt);
}
