<template>
    <div>
        <form class="col-md-12 person-create">
            <div class="form-group">
                <label>Име</label>
                <input type="text" class="form-control" v-model="firstName" />
            </div>
            <div class="form-group">
                <label>Презиме</label>
                <input type="text" class="form-control" v-model="middleName" />
            </div>
            <div class="form-group">
                <label>Фамилия</label>
                <input type="text" class="form-control" v-model="lastName" />
            </div>
            <div class="form-group text-center">
                <button class="btn btn-primary">Save</button>
            </div>
            <div class="clearfix"></div>
        </form>
    </div>
</template>

<script>
    export default {
        data: function () {
            return {
                firstName: "First Name",
                middleName: "Middle Name",
                lastName: "Last Name"
            }
        },
        mounted: function () {
            console.log('Component uid: ', this._uid);
        }
    }
</script>

<style scoped>
    form.person-create {
        background-color: lightgray;
        padding-top: 15px;
        padding-bottom: 1px;
    }
</style>