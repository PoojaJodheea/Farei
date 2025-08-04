namespace FormRequest.Models
{
    public static class StatusClass
    {
        public static string GetStatusClass(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "table-pending";

            return status.Trim().ToLower() switch
            {
                "pending" => "table-pending",
                "accepted" => "table-accept",
                "accept" => "table-accept",
                "rejected" => "table-reject",
                "reject" => "table-reject",

                "transit request" => "table-pending",
                "pending request" => "table-pending",
                "final request" => "table-pending",
                "onsite request" => "table-pending",

                "transitting" => "table-accept-transit",
                "reject transit" => "table-reject-transit",
                "accept transit" => "table-accept-transit",

                "accept onsite" => "table-accept-onsite",
                "reject onsite" => "table-reject-onsite",

                "repairing" => "table-repairing",
                "start repairing" => "table-repairing",

                "complete" => "table-complete",
                "send back" => "table-sendback",
                "return" => "table-complete",
                "closed" => "table-complete",

                _ => "table-pending"
            };
        }
    }
}
