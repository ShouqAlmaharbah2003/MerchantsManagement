document.addEventListener('DOMContentLoaded', function () {
    // ??? ????? ?? ??? Accept-Language ?? ??? ???????
    const acceptLanguage = window.navigator.language || 'en-US';
    const lang = acceptLanguage.startsWith('ar') ? 'ar-SA' : 'en-US';
    document.documentElement.lang = lang;

    // ?????? ??????
    const translations = {
        'ar-SA': {
            'Summary': '????',
            'Description': '?????',
            'Parameters': '?????????',
            'Responses': '??????',
            'Try it out': '??? ????',
            'Execute': '?????',
            'Clear': '???',
            'Code': '?????',
            'Example Value': '???? ????',
            'Server response': '?? ??????',
            'Authorization': '???????',
            'Schemas': '????????'
        },
        'en-US': {
            'Summary': 'Summary',
            'Description': 'Description',
            'Parameters': 'Parameters',
            'Responses': 'Responses',
            'Try it out': 'Try it out',
            'Execute': 'Execute',
            'Clear': 'Clear',
            'Code': 'Code',
            'Example Value': 'Example Value',
            'Server response': 'Server response',
            'Authorization': 'Authorization',
            'Schemas': 'Schemas'
        }
    };

    // ????? ??????? ????? ??? ?????
    const selectedTranslations = translations[lang];
    Object.keys(selectedTranslations).forEach(key => {
        document.querySelectorAll(`.swagger-ui :not(.translated):contains('${key}')`).forEach(element => {
            element.textContent = selectedTranslations[key];
            element.classList.add('translated');
        });
    });

    // ????? ????? ???????
    const titleElement = document.querySelector('.swagger-ui .info .title');
    if (titleElement) {
        titleElement.textContent = lang === 'ar-SA' ? '????? ????? ??????' : 'Merchants Management API';
    }
});