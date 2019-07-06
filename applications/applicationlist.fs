module ApplicationList
open Ft
let applications = []
let addApplication  (app:seq<ApplicationResource>) =
    let applications = app::applications
    applications