function PersonModel(obj) {
    var self = this;
    //[{
    //    "patientId": { "personId": 6, "practiceId": 4 }, "pidTypeCode": "Egn", "pidTypes": null, "identifier": "11112222",
    //    "firstName": "Стефан", "middleName": "Йорданов", "lastName": "Генов"
    //}]

    self.PatientId = ko.observable({});
    self.PatientId().PersonId = ko.observable(obj && obj.patientId && obj.patientId.personId || undefined);
    self.PatientId().PracticeId = ko.observable(obj && obj.patientId && obj.patientId.practiceId || undefined);
    //self.PracticeId = ko.observable(obj && obj.patientId && obj.patientId.practiceId || undefined);

    self.PidTypeCode = ko.observable(obj && obj.pidTypeCode || undefined);

    self.PidTypeCode = ko.observable(obj && obj.pidTypeCode || undefined);
    self.Identifier = ko.observable(obj && obj.identifier || undefined);
    self.FirstName = ko.observable(obj && obj.firstName || "");
    self.LastName = ko.observable(obj && obj.lastName || "");
}

function PersonListModel(obj) {
    var self = this;
    self.Items = ko.observableArray([]);

    self.Set = function (data) {
        $.each(data, function () {
            self.Items.push(new PersonModel(this));
        });
    };
}