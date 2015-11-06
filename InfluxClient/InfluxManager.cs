﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfluxClient
{
    public class InfluxManager
    {
        #region Constructor and private properties

        /// <summary>
        /// The base url for API calls
        /// </summary>
        private string _baseUrl = "";

        /// <summary>
        /// The influxDB database to use
        /// </summary>
        private string _database = "";

        /// <summary>
        /// Creates a new Influx manager
        /// </summary>
        /// <param name="influxEndpoint">The influxdb endpoint, including the port (if any)</param>
        /// <param name="database">The database to write to</param>
        public InfluxManager(string influxEndpoint, string database)
        {
            //  If the endpoint has a trailing backslash, remove it:
            if(influxEndpoint.EndsWith("/"))
            { influxEndpoint = influxEndpoint.Remove(influxEndpoint.LastIndexOf("/")); }

            //  Set the base url and database:
            _baseUrl = influxEndpoint;
            _database = database;
        } 

        #endregion

        #region API helpers

        /// <summary>
        /// Write a measurement to the InfluxDB database
        /// </summary>
        /// <param name="m">The measurement to write.  It must have at least one field specified</param>
        /// <returns>An awaitable Task containing the HttpResponseMessage returned from the InfluxDB server</returns>
        async public Task<HttpResponseMessage> Write(Measurement m)
        {
            //  Make sure the measurement has at least one field:
            if(!(m.BooleanFields.Any() 
                || m.FloatFields.Any() 
                || m.IntegerFields.Any() 
                || m.StringFields.Any()))
            {
                throw new ArgumentException("Measurements need at least one field value");
            }

            //  Create our url to post data to
            string url = string.Format("{0}/write?db={1}", _baseUrl, _database);

            //  Create our data to post:
            HttpContent content = new StringContent(LineProtocol.Format(m));

            //  Make an async call to get the response
            HttpClient client = new HttpClient();
            return await client.PostAsync(url, content);
        }

        async public Task<HttpResponseMessage> Write(List<Measurement> listOfMeasurements)
        {
            return null;
        }

        #endregion
    }
}
