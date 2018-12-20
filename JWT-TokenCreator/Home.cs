using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JWT_TokenCreator
{
    public partial class Home : Form
    {
        private IDictionary<string, string> tokenItems;

        public Home()
        {
            tokenItems = new Dictionary<string, string>();
            InitializeComponent();
        }

        private void btnGenerateToken_Click(object sender, EventArgs e)
        {
            txtToken.Text = GenerateToken(tokenItems);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtKey.Text != "" && txtValue.Text != "")
            {
                tokenItems.Add(txtKey.Text, txtValue.Text);
                txtToken.Text = $"{txtToken.Text}{txtKey.Text} - {txtValue.Text}{Environment.NewLine}";
                txtKey.Text = string.Empty;
                txtValue.Text = string.Empty;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtKey.Text = string.Empty;
            txtValue.Text = string.Empty;
            txtToken.Text = string.Empty;
            tokenItems.Clear();
        }

        private string GenerateToken(IDictionary<string, string> keyValues)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddDays(7);

            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity();

            foreach (var item in keyValues)
            {
                claimsIdentity.AddClaim(new Claim(item.Key, item.Value));
            }

            const string secretKey = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            //create the jwt
            var token = tokenHandler.CreateJwtSecurityToken(issuer: "http://localhost:8080",
                                                            audience: "http://localhost:8080",
                                                            subject: claimsIdentity,
                                                            notBefore: issuedAt,
                                                            expires: expires,
                                                            signingCredentials: signingCredentials);

            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
