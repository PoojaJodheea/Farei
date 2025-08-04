// Function to return class based on status value
window.getStatusClass = function (status) {
    if (!status) return "table-pending";
    status = status.trim().toLowerCase();

    switch (status) {
        case "pending":
        case "pending request":
        case "final request":
        case "transit request":
        case "onsite request":
            return "table-pending";
        case "accepted":
        case "accept":
            return "table-accept";
        case "rejected":
        case "reject":
            return "table-reject";
        case "transitting":
        case "accept transit":
            return "table-accept-transit";
        case "reject transit":
            return "table-reject-transit";
        case "accept onsite":
            return "table-accept-onsite";
        case "reject onsite":
            return "table-reject-onsite";
        case "repairing":
        case "start repairing":
            return "table-repairing";
        case "send back":
            return "table-sendback";
        case "return":
        case "complete":
        case "closed":
            return "table-complete";
        default:
            return "table-pending";
    }
};

// Global Accept/Reject handler
$(document).off("click", ".action-btn").on("click", ".action-btn", function (e) {
    e.preventDefault();
    const button = $(this);
    const id = button.data("id");
    const action = button.data("action");

    if (!confirm(`Are you sure you want to ${action} this request?`)) {
        // If clicked Cancel, do nothing — button stays enabled
        return;
    }

    $.ajax({
        url: "/FormReqDb/UpdateStatus",
        type: "POST",
        data: { id, actionType: action },
        success: function (response) {
            if (response.success) {
                button.prop("disabled", true);
                $(".status-cell[data-id='" + id + "']")
                    .text(response.newStatus)
                    .removeClass()
                    .addClass("status-cell " + getStatusClass(response.newStatus.toLowerCase()));
            } else {
                alert(response.message || "An error occurred.");
            }
        },
        error: function () {
            alert("Server error updating status.");
        }
    });
});
