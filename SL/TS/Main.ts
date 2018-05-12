
require.config({
    shim: {
        "bootstrap": { "deps": ['jquery'] },
        "jqueryui": { "deps": ['jquery'] },
        "jquery-validation": { "deps": ['jquery'] }

    },
    paths: {
        "knockout": "knockout-3.4.2",
        "bootstrap": "bootstrap.min",
        "popper":"popper.min",
        "jquery": "jquery-3.0.0.min",
        "jqueryui": "jquery-ui-1.12.1.min",
        "bindingHandler": "bindingHandler",
        "datepicker": "datepicker",
        "utils":"utils",
        "knockout-jqAutocomplete": "knockout-jqAutocomplete",
        "knockout-jqueryui": "knockout-jqueryui",
        "jquery-validation":"validation.min"
    },
    baseUrl: 'http://membership.nube.org.my/Scripts/'
    //baseUrl: 'http://localhost/MembershipTest/Scripts/'
});