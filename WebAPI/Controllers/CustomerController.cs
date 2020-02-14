﻿using Model;
using CustomerDB;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class CustomerController : ApiController
    {        
        // Checks the username, if its a known endpoint then extract the username from the token and validate. 
        // In this example the token has been generated by the APIGateway service
        [HttpGet]
        public HttpResponseMessage Validate(string token, string username)
        {
 
            string tokenUsername = TokenManager.ValidateToken(token);

            if (!username.Equals(tokenUsername))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "The JSON Web Token failed validation.");
            }

            CustomerManager customerManager = new CustomerManager();
            return Request.CreateResponse(HttpStatusCode.OK, customerManager.GetAllCustmers());

        }

    }
}