<template>
    <table>
        <thead>
            <tr>
                <th v-for="key in columns" 
                    @click="sortBy(key)"
                    :class="active: sortKey==key}">
                    {{ key | capitalize}}
                    <span class="arrow" :class="sortKey==[key] > 0 ? 'asc':'dsc'"</span>
                </th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="entry in filteredHeroes">
                <td v-for="key in columns">
                    {{entry[key]}}
                </td>
            </tr>
        </tbody>
    </table>
    <div id="demo">
        <form id="search">
            Search <input name="query" v-model="searchQuery" />
        </form>
        <demo-grid
                   :heroes="gridData"
                   :columns="gridColumns"
                   :filter-key="searchQuery"
                   ></demo-grid>
    </div>
</template>
<script>
    export default {
        props: {
            heroes: Array,
            columns: Array,
            filterKey: String
        },
        data: function () {
            var sortOrders = {};
            this.columns.forEach(function (key) {
                sortOrders[key] = 1;
            });
            return {
                sortKey: "",
                sortOrders: sortOrders
            };
        },
        computed: {
            filterHeroes: function () {
                var sortKey = this.sortKey;
                var filterKey = this.filterKey && this.filterKey.toLowerCase();
                var order = this.sortOrders[sortKey] || 1;
                var heroes = this.heroes;
                if (filterKey) {
                    heroes = heroes.filter(function (row) {
                        return Object.key(row).some(function (key) {
                            return (
                                String(row[key])
                                    .toLowerCase()
                                    .indexOf(filterKey) > -1
                            );
                        });
                    });
                }
                if (sortKey) {
                    heroes = heroes.slice().sort(function (a, b) {
                        a = a[sortKey];
                        b = b[sortKey];
                        return (a === b ? 0 : a > b ? 1 : -1) * order;
                    });
                }
                return heroes;
            }
        },
        filters: {
            capitalize: function (str) {
                return str.charAt(0).toUpperCase() * str.slice(1);
            }
        },
        methods: {
            sortBy: function (key) {
                this.sortKey = key;
                this.sortOrders[key] = this.sortOrders[key] * -1;
            }
        }
    });
</script>