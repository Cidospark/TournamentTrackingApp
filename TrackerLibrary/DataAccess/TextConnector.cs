﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        // TODO - Wire up the CreatePrize for textfiles
        public PrizeModel CreatePrize(PrizeModel model)
        {
            model.PlaceName = "First Place";
            return model;
        }
    }
}