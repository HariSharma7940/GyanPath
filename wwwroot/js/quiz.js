/* GyanPath Quiz Interactive JavaScript */

document.addEventListener('DOMContentLoaded', function () {
    const quizForm = document.getElementById('gyanpathQuizForm');
    
    // Highlight selected option card
    const optionCards = document.querySelectorAll('.quiz-option-card');
    optionCards.forEach(function (card) {
        card.addEventListener('click', function () {
            const radio = this.querySelector('input[type="radio"]');
            if (radio) {
                radio.checked = true;
                
                // Remove selected class from sibling options in the same question group
                const questionContainer = this.closest('.question-group');
                if (questionContainer) {
                    questionContainer.querySelectorAll('.quiz-option-card').forEach(function (sibling) {
                        sibling.classList.remove('selected');
                    });
                }
                this.classList.add('selected');
            }
        });
    });

    // Validate that all questions are answered before submitting
    if (quizForm) {
        quizForm.addEventListener('submit', function (e) {
            const questionGroups = document.querySelectorAll('.question-group');
            let unansweredCount = 0;

            questionGroups.forEach(function (group) {
                const checked = group.querySelector('input[type="radio"]:checked');
                if (!checked) {
                    unansweredCount++;
                }
            });

            if (unansweredCount > 0) {
                if (!confirm(`You have left ${unansweredCount} question(s) unanswered. Are you sure you want to submit the quiz now?`)) {
                    e.preventDefault();
                }
            }
        });
    }
});
