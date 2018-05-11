<template>
    <div>
        <h2>{{title}}</h2>
        <person></person>
    </div>
</template>
<script>
    import person from './Person.vue';

    export default {
        components: {
            person
        },
        data() {
            return {
                title: 'This is the main page title'
            }
        }
    }
</script>