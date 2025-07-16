namespace FormRequest.Models{ 
    public static class StatusClass   //helper class to convert status strings into css class names
    {
        public static string GetStatusClass(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "table-pending";

            return status.Trim().ToLower() switch
            {
                "accepted" => "table-accept",
                "reject" => "table-reject",
                "transit" => "table-pending",
                "onsite" => "table-pending",
                "accept transit" => "table-accept-transit",
                "reject transit" => "table-reject-transit",
                "accept onsite" => "table-accept-onsite",
                "reject onsite" => "table-reject-onsite",
                "complete" => "table-complete",
                "repairing" => "table-repairing",
                "send back" => "table-sendback",
                _ => "table-pending"
            };
        }
    }
}

