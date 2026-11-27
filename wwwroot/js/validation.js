/* GyanPath Custom Form Validation JavaScript */

document.addEventListener('DOMContentLoaded', function () {
    // Registration Form Password Confirmation Validation
    const regForm = document.getElementById('gyanpathRegisterForm');
    if (regForm) {
        regForm.addEventListener('submit', function (e) {
            const password = document.getElementById('regPassword');
            const confirmPassword = document.getElementById('regConfirmPassword');
            const matchError = document.getElementById('passwordMatchError');

            if (password && confirmPassword) {
                if (password.value !== confirmPassword.value) {
                    e.preventDefault();
                    if (matchError) {
                        matchError.textContent = "Passwords do not match!";
                        matchError.classList.remove('d-none');
                    }
                    confirmPassword.classList.add('is-invalid');
                } else {
                    if (matchError) {
                        matchError.classList.add('d-none');
                    }
                    confirmPassword.classList.remove('is-invalid');
                }
            }
        });
    }
});
