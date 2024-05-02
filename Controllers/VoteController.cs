using BisleriumPvtLtd.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BisleriumPvtLtd.Models;


namespace BisleriumPvtLtd.Controllers
{
    public class VoteController
    {
        private readonly BisleriumPvtLtdContext _context;


        public VoteController(BisleriumPvtLtdContext context)
        {
            _context = context;


        }

    }
}
