
$(document).ready(function () {
    $("#loginForm").submit(function (event) {
        event.preventDefault(); // Prevent traditional form submission

        var formData = {
            PhoneNumber: $("#PhoneNumber").val(),
            Password: $("#Password").val(),
            IsPersist: $("#IsPersist").is(":checked")
        };

        $.ajax({
            url: "/Account/Login",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify(formData),
            success: function (response) {
                if (response.success) {
                    // Store JWT token in localStorage or cookie
                    localStorage.setItem("authToken", response.token);

                    // Redirect to dashboard or homepage
                    window.location.href = "/Dashboard/Index";
                } else {
                    $("#loginError").text(response.message).show();
                }
            },
            error: function (xhr) {
                $("#loginError").text("Invalid login credentials.").show();
            }
        });
    });
});
