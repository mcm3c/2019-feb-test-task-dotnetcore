http://localhost:3000/odata/Locations


{"@odata.context":"http://localhost:3000/odata/$metadata#Locations","value":[{"ID":1,"Name":"Newcastle","State":"NSW"},{"ID":2,"Name":"Gosford","State":"NSW"},{"ID":3,"Name":"Sydney","State":"NSW"},{"ID":4,"Name":"Brisbane","State":"QLD"},{"ID":5,"Name":"Melbourne","State":"VIC"},{"ID":6,"Name":"Perth","State":"WA"}]}


http://localhost:3000/odata/Locations?$expand=Jobs

{"@odata.context":"http://localhost:3000/odata/$metadata#Locations(Jobs())","value":[{"ID":1,"Name":"Newcastle","State":"NSW","Jobs":[]},{"ID":2,"Name":"Gosford","State":"NSW","Jobs":[]},{"ID":3,"Name":"Sydney","State":"NSW","Jobs":[{"ID":1,"Name":"job 1","LocationID":3},{"ID":4,"Name":"job 4","LocationID":3}]},{"ID":4,"Name":"Brisbane","State":"QLD","Jobs":[{"ID":2,"Name":"job 2","LocationID":4},{"ID":3,"Name":"job 3","LocationID":4}]},{"ID":5,"Name":"Melbourne","State":"VIC","Jobs":[]},{"ID":6,"Name":"Perth","State":"WA","Jobs":[]}]}
