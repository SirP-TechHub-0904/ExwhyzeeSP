using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ExwhyzeeSP.Core.Models;
using ExwhyzeeSP.Data.Contexts;

namespace ExwhyzeeSP.Data.Identity
{
    public class LegacyUserStore : UserStore<ApplicationUser, IdentityRole, ApplicationDbContext, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityUserToken<string>, IdentityRoleClaim<string>>
    {
        public LegacyUserStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {
        }

        public override Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.FirstOrDefaultAsync(u => u.UserName == normalizedUserName, cancellationToken);
        }

        public override Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);
        }

        protected override async Task<IdentityRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            return await Context.Roles.FirstOrDefaultAsync(r => r.Name == normalizedRoleName, cancellationToken);
        }

        public override async Task<bool> IsInRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await Context.Set<IdentityUserRole<string>>().FirstOrDefaultAsync(ur => ur.RoleId.Equals(role.Id) && ur.UserId.Equals(user.Id), cancellationToken);
                return userRole != null;
            }
            return false;
        }

        public override async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));

            var userId = user.Id;
            var query = from userRole in Context.Set<IdentityUserRole<string>>()
                        join role in Context.Roles on userRole.RoleId equals role.Id
                        where userRole.UserId.Equals(userId)
                        select role.Name;

            return await query.ToListAsync(cancellationToken);
        }

        public override async Task AddToRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role == null)
            {
                throw new InvalidOperationException($"Role {normalizedRoleName} does not exist.");
            }
            Context.Set<IdentityUserRole<string>>().Add(new IdentityUserRole<string> { UserId = user.Id, RoleId = role.Id });
        }

        public override async Task RemoveFromRoleAsync(ApplicationUser user, string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var userRole = await Context.Set<IdentityUserRole<string>>().FirstOrDefaultAsync(r => r.RoleId.Equals(role.Id) && r.UserId.Equals(user.Id), cancellationToken);
                if (userRole != null)
                {
                    Context.Set<IdentityUserRole<string>>().Remove(userRole);
                }
            }
        }

        public override async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            if (string.IsNullOrWhiteSpace(normalizedRoleName)) throw new ArgumentException(nameof(normalizedRoleName));

            var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
            if (role != null)
            {
                var query = from userRole in Context.Set<IdentityUserRole<string>>()
                            join user in Users on userRole.UserId equals user.Id
                            where userRole.RoleId.Equals(role.Id)
                            select user;

                return await query.ToListAsync(cancellationToken);
            }
            return new List<ApplicationUser>();
        }

        public override Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user.UserName);
        }

        public override Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override Task<string> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(user.Email);
        }

        public override Task SetNormalizedEmailAsync(ApplicationUser user, string normalizedEmail, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        protected override Task<IdentityUserToken<string>?> FindTokenAsync(ApplicationUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            return Task.FromResult<IdentityUserToken<string>?>(null);
        }

        protected override Task AddUserTokenAsync(IdentityUserToken<string> token)
        {
            return Task.CompletedTask;
        }

        protected override Task RemoveUserTokenAsync(IdentityUserToken<string> token)
        {
            return Task.CompletedTask;
        }
    }

    public class LegacyRoleStore : RoleStore<IdentityRole, ApplicationDbContext, string, IdentityUserRole<string>, IdentityRoleClaim<string>>
    {
        public LegacyRoleStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {
        }

        public override Task<IdentityRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();
            return Roles.FirstOrDefaultAsync(r => r.Name == normalizedRoleName, cancellationToken);
        }

        public override Task<string?> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(role.Name);
        }

        public override Task SetNormalizedRoleNameAsync(IdentityRole role, string? normalizedName, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override Task<IList<Claim>> GetClaimsAsync(IdentityRole role, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IList<Claim>>(new List<Claim>());
        }

        public override Task AddClaimAsync(IdentityRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public override Task RemoveClaimAsync(IdentityRole role, Claim claim, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
