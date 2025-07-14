using CommonLib.Dtos.Requests;
using CommonLib.Dtos.Responses;
using CommonLib.Interfaces;
using DataLib.Interfaces;
using DataLib.Models;
using NHibernate;
using Serilog;
using System;
using System.Threading.Tasks;

namespace CommonLib.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IJwtService _jwtService;
        private readonly ISession _session;

        public UserService(IUserRepository repository, IJwtService jwtService, ISession session)
        {
            _repository = repository;
            _jwtService = jwtService;
            _session = session;
        }

        public async Task<UserWithTokenResponse> LoginAsync(LoginRequest dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
            {
                Log.Warning("Login attempt with empty username or password.");
                throw new ArgumentException("Username and password are required.");
            }

            var user = await _repository.GetByUsernameAsync(dto.Username).ConfigureAwait(false);
            if (user == null)
            {
                Log.Warning("Login failed: User {Username} not found.", dto.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                Log.Warning("Login failed: Invalid password for username {Username}.", dto.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

                Log.Information("Generating token for user {Username}", dto.Username);
                var token = await _jwtService.GenerateTokenAsync(user.Username).ConfigureAwait(false);
                Log.Information("User {Username} logged in successfully.", dto.Username);

                return new UserWithTokenResponse
                {
                    Id = user.Id,
                    Username = user.Username,
                    Token = token
                };
        }

        public async Task<User> RegisterAsync(RegisterRequest dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Email))
            {
                Log.Warning("Registration attempt with empty username, password, or email.");
                throw new ArgumentException("Username, password, and email are required.");
            }

            var existingUser = await _repository.GetByUsernameAsync(dto.Username).ConfigureAwait(false);
            if (existingUser != null)
            {
                Log.Warning("Registration failed: Username {Username} already exists.", dto.Username);
                throw new InvalidOperationException("Username already exists.");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow
            };

                Log.Information("Registering user {Username}", dto.Username);
                // التأكد من أن الجلسة لا تزال نشطة
                if (!_session.IsOpen)
                {
                    Log.Error("Session is closed during registration for user {Username}", dto.Username);
                    throw new InvalidOperationException("Session is not open.");
                }

                var transaction = _session.BeginTransaction();
                
                    await _repository.SaveAsync(user).ConfigureAwait(false);
                await transaction.CommitAsync();
                    Log.Information("User {Username} registered successfully.", dto.Username);
                return user;
        }
    }
}